namespace Enterprise.Framework.Application.Features.Assets.Queries;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Enterprise.Framework.Application.Common.Mappings;
using Enterprise.Framework.Application.Common.Extensions;

public sealed record GetAssetsQuery : IRequest<PagedResult<AssetDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public string? SortOrder { get; init; }
    public string? FiltersJson { get; init; }
}

public class GetAssetsQueryHandler : IRequestHandler<GetAssetsQuery, PagedResult<AssetDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAssetsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<AssetDto>> Handle(GetAssetsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.GetDbSet<Asset>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(x => 
                x.Name.ToLower().Contains(searchTerm) || 
                x.SerialNumber.ToLower().Contains(searchTerm));
        }

        query = query.ApplyGridOptions(request.SortOrder, request.FiltersJson);

        return await query.PaginatedProjectToAsync<AssetDto>(request.PageNumber, request.PageSize, _mapper.ConfigurationProvider);
    }
}


