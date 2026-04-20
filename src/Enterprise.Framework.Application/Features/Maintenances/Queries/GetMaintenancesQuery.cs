namespace Enterprise.Framework.Application.Features.Maintenances.Queries;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Enterprise.Framework.Application.Common.Mappings;

public sealed record GetMaintenancesQuery : IRequest<PagedResult<MaintenanceDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public string? SortOrder { get; init; }
}

public class GetMaintenancesQueryHandler : IRequestHandler<GetMaintenancesQuery, PagedResult<MaintenanceDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetMaintenancesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<MaintenanceDto>> Handle(GetMaintenancesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.GetDbSet<Maintenance>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(x => x.Notes.ToLower().Contains(searchTerm));
        }

        query = request.SortOrder switch
        {
            "scheduledDate_desc" => query.OrderByDescending(x => x.ScheduledDate),
            "scheduledDate_asc" => query.OrderBy(x => x.ScheduledDate),
            "status_desc" => query.OrderByDescending(x => x.Status),
            "status_asc" => query.OrderBy(x => x.Status),
            "id_asc" => query.OrderBy(x => x.Id),
            _ => query.OrderByDescending(x => x.Id)
        };

        return await query.PaginatedProjectToAsync<MaintenanceDto>(request.PageNumber, request.PageSize, _mapper.ConfigurationProvider);
    }
}
