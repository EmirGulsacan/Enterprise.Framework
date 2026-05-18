namespace Enterprise.Framework.API.Endpoints.RuleEngine;

public class EvaluateOperationRequest
{
    public string OperationCode { get; set; } = default!;
    public Dictionary<string, object> ContextData { get; set; } = new();
}
