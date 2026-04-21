namespace Enterprise.Framework.Application.Features.Employees.Queries;

using Enterprise.Framework.Application.Common.Mappings;
using Enterprise.Framework.Domain.Entities;

public sealed record EmployeeDto : IMapFrom<Employee>
{
    public long Id { get; init; }
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string Department { get; init; } = default!;
}
