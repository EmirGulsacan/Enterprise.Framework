namespace Enterprise.Framework.API.Endpoints.RuleEngine;

using Enterprise.Framework.API.Common;
using Enterprise.Framework.Application.RuleEngine;

public class RuleEvaluationEndpoints : IEndpointDefinition
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/rules")
            .WithTags("RuleEngine");

        group.MapPost("/evaluate", async (
            EvaluateRuleRequest request,
            IRuleEvaluator evaluator,
            IServiceProvider sp) =>
        {
            var context = new RuleContext();
            foreach (var kv in request.ContextData)
                context[kv.Key] = kv.Value;

            var enricher = sp.GetService<IContextEnricher>();
            if (enricher is not null)
                context = await enricher.EnrichAsync(request.RuleCode, context);

            var result = await evaluator.EvaluateAsync(request.RuleCode, context);

            if (!result.IsSuccess && result.TriggeredRules.Count == 0 && result.Errors.Any(e => e.Contains("not found")))
                return Results.NotFound(new { message = result.Errors[0] });

            return Results.Ok(result);
        });

        group.MapPost("/evaluate-operation", async (
            EvaluateOperationRequest request,
            IRuleEvaluator evaluator,
            IServiceProvider sp) =>
        {
            var context = new RuleContext();
            foreach (var kv in request.ContextData)
                context[kv.Key] = kv.Value;

            var enricher = sp.GetService<IContextEnricher>();
            if (enricher is not null)
                context = await enricher.EnrichAsync(request.OperationCode, context);

            var result = await evaluator.EvaluateOperationAsync(request.OperationCode, context);

            if (!result.IsSuccess && result.TriggeredRules.Count == 0 && result.Errors.Any(e => e.Contains("not found")))
                return Results.NotFound(new { message = result.Errors[0] });

            return Results.Ok(result);
        });
    }
}
