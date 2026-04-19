namespace Enterprise.Framework.Application.Labors.Queries;

using Enterprise.Framework.Application.Common.Mappings;
using Enterprise.Framework.Domain.Entities;

public record LaborDto : IMapFrom<Labor>
{
    public int Id { get; init; }
    public int MaintenanceId { get; init; }
    public int EmployeeId { get; init; }
    public decimal HoursWorked { get; init; }
    public decimal HourlyRate { get; init; }
}
