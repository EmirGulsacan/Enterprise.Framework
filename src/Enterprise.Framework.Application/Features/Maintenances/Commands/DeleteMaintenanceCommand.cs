namespace Enterprise.Framework.Application.Features.Maintenances.Commands;

using Enterprise.Framework.Application.Common.Exceptions;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using MediatR;

public sealed record DeleteMaintenanceCommand(long Id) : IRequest<Unit>;

public class DeleteMaintenanceCommandHandler : IRequestHandler<DeleteMaintenanceCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteMaintenanceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GetDbSet<Maintenance>().FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Maintenance), request.Id);
        }

        _context.GetDbSet<Maintenance>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
