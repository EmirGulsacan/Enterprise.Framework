namespace Enterprise.Framework.Application.Maintenances.Queries;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Enterprise.Framework.Application.Common.Mappings;

public record GetMaintenancesQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<MaintenanceDto>>;

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
        var query = _context.Maintenances.AsNoTracking();
        return await query.PaginatedProjectToAsync<MaintenanceDto>(request.PageNumber, request.PageSize, _mapper.ConfigurationProvider);
    }
}
