namespace Enterprise.Framework.Application.Features.Maintenances.Commands;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Common.Enums;
using Enterprise.Framework.Domain.Entities;
using MediatR;

public sealed record CreateMaintenanceCommand : IRequest<long>
{
    public long AssetId { get; init; }
    public DateTime ScheduledDate { get; init; }
    public DateTime? CompletedDate { get; init; }
    public string Notes { get; init; } = default!;
    public MaintenanceStatus Status { get; init; } = MaintenanceStatus.Scheduled;
}

public class CreateMaintenanceCommandHandler : IRequestHandler<CreateMaintenanceCommand, long>
{
    private readonly IApplicationDbContext _context;

    public CreateMaintenanceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<long> Handle(CreateMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var entity = new Maintenance
        {
            AssetId = request.AssetId,
            ScheduledDate = request.ScheduledDate,
            CompletedDate = request.CompletedDate,
            Notes = request.Notes,
            Status = request.Status
        };

        _context.GetDbSet<Maintenance>().Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
