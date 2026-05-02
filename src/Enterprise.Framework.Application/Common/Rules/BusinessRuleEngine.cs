namespace Enterprise.Framework.Application.Common.Rules;

using Enterprise.Framework.Application.Common.Exceptions;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class BusinessRuleEngine : IBusinessRuleEngine
{
    public async Task CheckAsync(CancellationToken cancellationToken, params IBusinessRule[] rules)
    {
        if (rules == null || rules.Length == 0) return;

        foreach (var rule in rules.OrderBy(r => r.Order))
        {
            if (await rule.IsBrokenAsync(cancellationToken))
            {
                throw new BusinessRuleException(rule);
            }
        }
    }
}
