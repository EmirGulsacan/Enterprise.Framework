namespace Enterprise.Framework.Application.Features.Labors.Queries;

using Enterprise.Framework.Application.Common.Mappings;
using Enterprise.Framework.Domain.Entities;

public sealed record LaborDto : IMapFrom<Labor>
{
    public int Id { get; init; }
    public int MaintenanceId { get; init; }
    public int EmployeeId { get; init; }
    public decimal HoursWorked { get; init; }
    public decimal HourlyRate { get; init; }
}
