namespace Enterprise.Framework.Domain.Common;

public interface IDateTimeProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}
