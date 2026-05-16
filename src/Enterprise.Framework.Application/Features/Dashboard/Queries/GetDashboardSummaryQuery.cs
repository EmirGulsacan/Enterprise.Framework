namespace Enterprise.Framework.Application.Features.Dashboard.Queries;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed record DashboardSummaryDto
{
    public int TotalUsers { get; init; }
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

        return new DashboardSummaryDto
        {
            TotalUsers = totalUsers
        };
    }
}
