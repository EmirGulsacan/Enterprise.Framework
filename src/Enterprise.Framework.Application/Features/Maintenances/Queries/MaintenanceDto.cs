namespace Enterprise.Framework.Application.Features.Maintenances.Queries;

using Enterprise.Framework.Application.Common.Mappings;
using Enterprise.Framework.Domain.Entities;

public sealed record MaintenanceDto : IMapFrom<Maintenance>
{
    public int Id { get; init; }
    public int AssetId { get; init; }
    public DateTime ScheduledDate { get; init; }
    public DateTime? CompletedDate { get; init; }
    public string Notes { get; init; } = default!;
    public bool IsCompleted { get; init; }
}
