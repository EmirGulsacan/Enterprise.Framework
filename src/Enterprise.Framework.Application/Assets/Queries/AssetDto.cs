namespace Enterprise.Framework.Application.Assets.Queries;

using Enterprise.Framework.Application.Common.Mappings;
using Enterprise.Framework.Domain.Entities;

public record AssetDto : IMapFrom<Asset>
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string SerialNumber { get; init; } = default!;
    public DateTime PurchaseDate { get; init; }
    public string Status { get; init; } = default!;
    public int? AssignedEmployeeId { get; init; }
}
