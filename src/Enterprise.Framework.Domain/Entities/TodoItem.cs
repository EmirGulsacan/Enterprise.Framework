using Enterprise.Framework.Domain.Common;

namespace Enterprise.Framework.Domain.Entities;

public sealed class TodoItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; set; }
}

