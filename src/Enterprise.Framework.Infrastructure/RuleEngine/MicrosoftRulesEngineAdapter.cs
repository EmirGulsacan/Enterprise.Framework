namespace Enterprise.Framework.Infrastructure.RuleEngine;

using System.Dynamic;
using System.Text.Json;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.RuleEngine;
using Enterprise.Framework.Domain.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RE = global::RulesEngine;

public class MicrosoftRulesEngineAdapter : IRuleEvaluator
{
    private readonly IApplicationDbContext _db;
    private readonly IMemoryCache _cache;

    public MicrosoftRulesEngineAdapter(IApplicationDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<RuleEvaluationResult> EvaluateAsync(string ruleCode, RuleContext context)
    {
        var cacheKey = $"rule_engine:{ruleCode}";

        if (!_cache.TryGetValue(cacheKey, out RE.RulesEngine? engine))
        {
            var rule = await _db.GetDbSet<RuleDefinition>()
                .AsNoTracking()
                .Where(r => r.Code == ruleCode && r.IsActive && !r.IsDeleted)
                .FirstOrDefaultAsync();

            if (rule is null)
                return new RuleEvaluationResult { IsSuccess = false, Errors = [$"Rule not found: {ruleCode}"] };

            var workflows = JsonSerializer.Deserialize<RE.Models.Workflow[]>(
                rule.WorkflowJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? [];

            engine = new RE.RulesEngine(workflows);

            _cache.Set(cacheKey, engine, TimeSpan.FromMinutes(30));
        }

        var expando = new ExpandoObject();
        var expandoDict = (IDictionary<string, object>)expando!;
        foreach (var kv in context)
            expandoDict[kv.Key] = kv.Value;

        var ruleResults = await engine!.ExecuteAllRulesAsync(ruleCode, expando);

        var result = new RuleEvaluationResult();

        foreach (var rr in ruleResults)
        {
            if (rr.IsSuccess)
                result.TriggeredRules.Add(rr.Rule.RuleName);
            else if (rr.ExceptionMessage is { Length: > 0 })
                result.Errors.Add($"{rr.Rule.RuleName}: {rr.ExceptionMessage}");
        }

        result.IsSuccess = result.Errors.Count == 0;

        return result;
    }

    public async Task<RuleEvaluationResult> EvaluateOperationAsync(string operationCode, RuleContext context)
    {
        var operation = await _db.GetDbSet<OperationDefinition>()
            .AsNoTracking()
            .Include(x => x.RuleMappings)
                .ThenInclude(x => x.RuleDefinition)
            .Where(x => x.Code == operationCode && x.IsActive && !x.IsDeleted)
            .FirstOrDefaultAsync();

        if (operation is null)
            return new RuleEvaluationResult { IsSuccess = false, Errors = [$"Operation not found: {operationCode}"] };

        var finalResult = new RuleEvaluationResult { IsSuccess = true };

        foreach (var mapping in operation.RuleMappings)
        {
            var ruleResult = await EvaluateAsync(mapping.RuleDefinition.Code, context);
            
            if (!ruleResult.IsSuccess)
                finalResult.IsSuccess = false;

            finalResult.TriggeredRules.AddRange(ruleResult.TriggeredRules);
            finalResult.Errors.AddRange(ruleResult.Errors);
        }

        return finalResult;
    }
}
