namespace Enterprise.Framework.Application.Features.Labors.Queries;

using Enterprise.Framework.Application.Common.Mappings;
using Enterprise.Framework.Domain.Entities;

public sealed record LaborDto : IMapFrom<Labor>
{
    public long Id { get; init; }
    public long MaintenanceId { get; init; }
    public long EmployeeId { get; init; }
    public decimal HoursWorked { get; init; }
    public decimal HourlyRate { get; init; }
}
