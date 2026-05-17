namespace Enterprise.Framework.API.Endpoints.RuleEngine;

public class EvaluateRuleRequest
{
    public string RuleCode { get; set; } = default!;
    public Dictionary<string, object> ContextData { get; set; } = new();
}
