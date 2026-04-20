namespace Enterprise.Framework.Application.Features.Identity.Queries;

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using Enterprise.Framework.Application.Features.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed record GetRolesQuery : IRequest<List<RoleDto>>;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, List<RoleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetRolesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        return await _context.GetDbSet<AppRole>()
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ProjectTo<RoleDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
