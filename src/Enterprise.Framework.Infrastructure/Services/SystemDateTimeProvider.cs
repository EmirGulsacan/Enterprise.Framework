namespace Enterprise.Framework.Infrastructure.Services;

using Enterprise.Framework.Domain.Common;

public sealed class SystemDateTimeProvider : IDateTimeProvider {
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}
