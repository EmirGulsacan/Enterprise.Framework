namespace Enterprise.Framework.Application.Assets.Queries;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Enterprise.Framework.Application.Common.Mappings;

public record GetAssetsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<AssetDto>>;

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
        var query = _context.Assets.AsNoTracking();
        return await query.PaginatedProjectToAsync<AssetDto>(request.PageNumber, request.PageSize, _mapper.ConfigurationProvider);
    }
}
