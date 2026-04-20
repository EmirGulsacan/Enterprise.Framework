namespace Enterprise.Framework.Application.Features.Labors.Queries;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Enterprise.Framework.Application.Common.Mappings;

public sealed record GetLaborsQuery : IRequest<PagedResult<LaborDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public string? SortOrder { get; init; }
}

public class GetLaborsQueryHandler : IRequestHandler<GetLaborsQuery, PagedResult<LaborDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetLaborsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<LaborDto>> Handle(GetLaborsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.GetDbSet<Labor>().AsNoTracking();

        query = request.SortOrder switch
        {
            "hoursWorked_desc" => query.OrderByDescending(x => x.HoursWorked),
            "hoursWorked_asc" => query.OrderBy(x => x.HoursWorked),
            "id_asc" => query.OrderBy(x => x.Id),
            _ => query.OrderByDescending(x => x.Id)
        };

        return await query.PaginatedProjectToAsync<LaborDto>(request.PageNumber, request.PageSize, _mapper.ConfigurationProvider);
    }
}
