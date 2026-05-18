namespace Enterprise.Framework.Application.RuleEngine;

public interface IRuleEvaluator
{
    Task<RuleEvaluationResult> EvaluateAsync(string ruleCode, RuleContext context);
    Task<RuleEvaluationResult> EvaluateOperationAsync(string operationCode, RuleContext context);
}
