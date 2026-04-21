namespace Enterprise.Framework.Application.Features.Dashboard.Queries;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Enterprise.Framework.Domain.Common.Enums;

public sealed record DashboardSummaryDto
{
    public int TotalUsers { get; init; }
    public int ActiveMaintenances { get; init; }
    public int TotalAssets { get; init; }
    public int PendingLabors { get; init; }
}

public sealed record GetDashboardSummaryQuery : IRequest<DashboardSummaryDto> { }

sealed class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly IApplicationDbContext _context;

    public GetDashboardSummaryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var totalUsers = await _context.GetDbSet<AppUser>().CountAsync(cancellationToken);
        
        var totalAssets = await _context.GetDbSet<Asset>().CountAsync(cancellationToken);
        
        var activeMaintenances = await _context.GetDbSet<Maintenance>()
            .CountAsync(m => m.Status != MaintenanceStatus.Completed, cancellationToken);
            
        var pendingLabors = await _context.GetDbSet<Labor>()
            .CountAsync(l => l.CreatedAtUtc >= DateTime.UtcNow.AddDays(-7), cancellationToken);

        return new DashboardSummaryDto
        {
            TotalUsers = totalUsers,
            TotalAssets = totalAssets,
            ActiveMaintenances = activeMaintenances,
            PendingLabors = pendingLabors
        };
    }
}
