namespace Enterprise.Framework.Domain.Entities;

using Enterprise.Framework.Domain.Common;

public class Document : BaseEntity, IAuditableEntity
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string RelatedEntityType { get; set; } = string.Empty; // e.g., "Asset", "Maintenance"
    public long RelatedEntityId { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedAtUtc { get; set; }
    public string? LastModifiedBy { get; set; }
}
