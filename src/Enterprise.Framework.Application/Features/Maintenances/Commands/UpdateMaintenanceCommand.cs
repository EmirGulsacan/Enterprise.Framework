namespace Enterprise.Framework.Application.Features.Maintenances.Commands;

using Enterprise.Framework.Application.Common.Exceptions;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Common.Enums;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed record UpdateMaintenanceCommand : IRequest<Unit>
{
    public long Id { get; init; }
    public long AssetId { get; init; }
    public DateTime ScheduledDate { get; init; }
    public DateTime? CompletedDate { get; init; }
    public string Notes { get; init; } = string.Empty;
    public MaintenanceStatus Status { get; init; }
}

public class UpdateMaintenanceCommandHandler : IRequestHandler<UpdateMaintenanceCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateMaintenanceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GetDbSet<Maintenance>().FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Maintenance), request.Id);
        }

        entity.AssetId = request.AssetId;
        entity.ScheduledDate = request.ScheduledDate;
        entity.CompletedDate = request.CompletedDate;
        entity.Notes = request.Notes;
        entity.Status = request.Status;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
