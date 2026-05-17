namespace Enterprise.Framework.Application.RuleEngine;

public class RuleEvaluationResult
{
    public bool IsSuccess { get; set; }
    public List<string> TriggeredRules { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}
