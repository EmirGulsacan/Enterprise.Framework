namespace Enterprise.Framework.Application.Features.Identity.Queries;

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Common.Mappings;
using Enterprise.Framework.Domain.Entities;
using Enterprise.Framework.Application.Features.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed record GetPermissionsQuery : IRequest<List<PermissionDto>>;

public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, List<PermissionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPermissionsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<PermissionDto>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        return await _context.GetDbSet<AppPermission>()
            .AsNoTracking()
            .Include(x => x.Module)
            .OrderBy(x => x.Module.Name)
            .ThenBy(x => x.Name)
            .ProjectTo<PermissionDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
