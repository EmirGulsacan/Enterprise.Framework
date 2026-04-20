namespace Enterprise.Framework.Application.Features.Employees.Commands;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using MediatR;

public sealed record CreateEmployeeCommand : IRequest<long>
{
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string Department { get; init; } = default!;
}

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, long>
{
    private readonly IApplicationDbContext _context;

    public CreateEmployeeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<long> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var entity = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Title = request.Title,
            Department = request.Department
        };

        _context.GetDbSet<Employee>().Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
