namespace Enterprise.Framework.Application.Features.Assets.Queries;

using Enterprise.Framework.Application.Common.Mappings;
using Enterprise.Framework.Domain.Entities;

public sealed record AssetDto : IMapFrom<Asset>
{
    public long Id { get; init; }
    public string Name { get; init; } = default!;
    public string SerialNumber { get; init; } = default!;
    public DateTime PurchaseDate { get; init; }
    public string Status { get; init; } = default!;
    public long? AssignedEmployeeId { get; init; }
}
