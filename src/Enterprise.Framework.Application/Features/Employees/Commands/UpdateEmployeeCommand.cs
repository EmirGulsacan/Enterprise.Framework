namespace Enterprise.Framework.Application.Features.Employees.Commands;

using Enterprise.Framework.Application.Common.Exceptions;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

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

    public UpdateEmployeeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GetDbSet<Employee>().FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Employee), request.Id);
        }

        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Email = request.Email;
        entity.Title = request.Title;
        entity.Department = request.Department;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
