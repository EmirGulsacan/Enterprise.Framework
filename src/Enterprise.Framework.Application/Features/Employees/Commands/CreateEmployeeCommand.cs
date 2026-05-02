namespace Enterprise.Framework.Application.Features.Employees.Commands;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Features.Employees.Rules;
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
    private readonly IBusinessRuleEngine _businessRuleEngine;

    public CreateEmployeeCommandHandler(IApplicationDbContext context, IBusinessRuleEngine businessRuleEngine)
    {
        _context = context;
        _businessRuleEngine = businessRuleEngine;
    }

    public async Task<long> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        await _businessRuleEngine.CheckAsync(cancellationToken,
            new EmployeeEmailMustBeUniqueRule(_context, request.Email, order: 1)
        );
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
