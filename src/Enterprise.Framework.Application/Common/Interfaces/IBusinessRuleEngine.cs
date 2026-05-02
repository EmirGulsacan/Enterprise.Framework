namespace Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

public interface IBusinessRuleEngine
{
    Task CheckAsync(CancellationToken cancellationToken, params IBusinessRule[] rules);
}
