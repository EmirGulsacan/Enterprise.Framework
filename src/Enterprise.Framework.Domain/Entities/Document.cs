namespace Enterprise.Framework.Domain.Entities;

using Enterprise.Framework.Domain.Common;

public class Document : AuditableEntity
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string RelatedEntityType { get; set; } = string.Empty;
    public long RelatedEntityId { get; set; }
}

