namespace Enterprise.Framework.Application.Common.Exceptions;

using Enterprise.Framework.Domain.Common;

public class BusinessRuleException : Exception
{
    public IBusinessRule BrokenRule { get; }

    public BusinessRuleException(IBusinessRule brokenRule) : base(brokenRule.Message)
    {
        BrokenRule = brokenRule;
    }

    public override string ToString()
    {
        return $"{BrokenRule.GetType().Name}: {BrokenRule.Message}";
    }
}
