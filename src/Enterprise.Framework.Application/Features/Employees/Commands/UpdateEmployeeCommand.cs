namespace Enterprise.Framework.Application.Features.Employees.Commands;

using Enterprise.Framework.Application.Common.Exceptions;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Common.Rules;
using Enterprise.Framework.Application.Features.Employees.Rules;
using Enterprise.Framework.Domain.Entities;
using MediatR;

public sealed record UpdateEmployeeCommand : IRequest<Unit>
{
    public long Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
}

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IBusinessRuleEngine _businessRuleEngine;

    public UpdateEmployeeCommandHandler(IApplicationDbContext context, IBusinessRuleEngine businessRuleEngine)
    {
        _context = context;
        _businessRuleEngine = businessRuleEngine;
    }

    public async Task<Unit> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        await _businessRuleEngine.CheckAsync(cancellationToken,
            new EntityMustExistRule<Employee>(_context, request.Id, order: 1),
            new EmployeeEmailMustBeUniqueRule(_context, request.Email, request.Id, order: 2)
        );

        var entity = await _context.GetDbSet<Employee>().FindAsync(new object[] { request.Id }, cancellationToken);

        entity!.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Email = request.Email;
        entity.Title = request.Title;
        entity.Department = request.Department;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
