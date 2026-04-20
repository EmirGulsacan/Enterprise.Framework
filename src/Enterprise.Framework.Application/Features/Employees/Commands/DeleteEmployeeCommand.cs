namespace Enterprise.Framework.Application.Features.Employees.Commands;

using Enterprise.Framework.Application.Common.Exceptions;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using MediatR;

public sealed record DeleteEmployeeCommand(long Id) : IRequest<Unit>;

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteEmployeeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GetDbSet<Employee>().FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Employee), request.Id);
        }

        _context.GetDbSet<Employee>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
