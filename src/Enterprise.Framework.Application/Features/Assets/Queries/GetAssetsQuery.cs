namespace Enterprise.Framework.Application.Features.Assets.Queries;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Enterprise.Framework.Application.Common.Mappings;

public sealed record GetAssetsQuery : IRequest<PagedResult<AssetDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public string? SortOrder { get; init; }
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

        query = request.SortOrder switch
        {
            "name_desc" => query.OrderByDescending(x => x.Name),
            "name_asc" => query.OrderBy(x => x.Name),
            "serialNumber_desc" => query.OrderByDescending(x => x.SerialNumber),
            "serialNumber_asc" => query.OrderBy(x => x.SerialNumber),
            "id_asc" => query.OrderBy(x => x.Id),
            _ => query.OrderByDescending(x => x.Id)
        };

        return await query.PaginatedProjectToAsync<AssetDto>(request.PageNumber, request.PageSize, _mapper.ConfigurationProvider);
    }
}
