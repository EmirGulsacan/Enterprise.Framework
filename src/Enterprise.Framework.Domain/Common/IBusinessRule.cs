namespace Enterprise.Framework.Domain.Common;

using System.Threading;
using System.Threading.Tasks;

public interface IBusinessRule
{
    int Order { get; }
    string ErrorCode { get; }
    string Message { get; }
    Task<bool> IsBrokenAsync(CancellationToken cancellationToken = default);
}
