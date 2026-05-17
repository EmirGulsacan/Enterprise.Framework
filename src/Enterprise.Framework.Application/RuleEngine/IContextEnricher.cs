namespace Enterprise.Framework.Application.RuleEngine;

public interface IContextEnricher
{
    Task<RuleContext> EnrichAsync(string ruleCode, RuleContext baseContext);
}
