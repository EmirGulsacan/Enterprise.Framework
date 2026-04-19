namespace Enterprise.Framework.Application.Common.Rules;

using Enterprise.Framework.Application.Common.Exceptions;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Common;

public class BusinessRuleEngine : IBusinessRuleEngine
{
    public async Task CheckAsync(IBusinessRule rule)
    {
        if (await rule.IsBrokenAsync())
        {
            throw new BusinessRuleException(rule);
        }
    }

    public async Task CheckAsync(params IBusinessRule[] rules)
    {
        foreach (var rule in rules)
        {
            if (await rule.IsBrokenAsync())
            {
                throw new BusinessRuleException(rule);
            }
        }
    }
}
