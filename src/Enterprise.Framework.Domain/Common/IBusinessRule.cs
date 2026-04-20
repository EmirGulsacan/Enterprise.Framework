namespace Enterprise.Framework.Domain.Common;

public interface IBusinessRule
{
    string Message { get; }
    Task<bool> IsBrokenAsync();
}
