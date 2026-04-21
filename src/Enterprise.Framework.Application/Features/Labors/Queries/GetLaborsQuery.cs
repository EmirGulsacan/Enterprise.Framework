using Microsoft.AspNetCore.Mvc;
namespace Enterprise.Framework.Application.Features.Labors.Queries;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Enterprise.Framework.Application.Common.Mappings;
using Enterprise.Framework.Application.Common.Extensions;

public sealed record GetLaborsQuery : IRequest<PagedResult<LaborDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public string? SortOrder { get; init; }
    public string? FiltersJson { get; init; }
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

        query = query.ApplyGridOptions(request.SortOrder, request.FiltersJson);

        return await query.PaginatedProjectToAsync<LaborDto>(request.PageNumber, request.PageSize, _mapper.ConfigurationProvider);
    }
}



