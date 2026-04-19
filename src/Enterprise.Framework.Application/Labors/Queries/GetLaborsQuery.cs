namespace Enterprise.Framework.Application.Labors.Queries;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Enterprise.Framework.Application.Common.Mappings;

public record GetLaborsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<LaborDto>>;

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
        var query = _context.Labors.AsNoTracking();
        return await query.PaginatedProjectToAsync<LaborDto>(request.PageNumber, request.PageSize, _mapper.ConfigurationProvider);
    }
}
