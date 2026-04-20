namespace Enterprise.Framework.Application.Features.Documents.Queries;

using Enterprise.Framework.Application.Common.Mappings;
using Enterprise.Framework.Domain.Entities;

public sealed record DocumentDto : IMapFrom<Document>
{
    public int Id { get; init; }
    public string FileName { get; init; } = default!;
    public string ContentType { get; init; } = default!;
    public string Path { get; init; } = default!;
    public string RelatedEntityType { get; init; } = default!;
    public int RelatedEntityId { get; init; }
}
