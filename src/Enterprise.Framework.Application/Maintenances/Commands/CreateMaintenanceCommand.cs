namespace Enterprise.Framework.Application.Maintenances.Commands;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using MediatR;

public record CreateMaintenanceCommand : IRequest<long>
{
    public int AssetId { get; init; }
    public DateTime ScheduledDate { get; init; }
    public DateTime? CompletedDate { get; init; }
    public string Notes { get; init; } = default!;
    public bool IsCompleted { get; init; }
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
            IsCompleted = request.IsCompleted
        };

        _context.Maintenances.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
