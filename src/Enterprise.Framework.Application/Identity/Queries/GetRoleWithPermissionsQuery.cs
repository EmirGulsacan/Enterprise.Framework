namespace Enterprise.Framework.Application.Identity.Queries;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Common.Exceptions;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

public record RoleWithPermissionsDto
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public List<long> PermissionIds { get; init; } = new();
}

public record GetRoleWithPermissionsQuery(long RoleId) : IRequest<RoleWithPermissionsDto>;

public class GetRoleWithPermissionsQueryHandler : IRequestHandler<GetRoleWithPermissionsQuery, RoleWithPermissionsDto>
{
    private readonly IApplicationDbContext _context;

    public GetRoleWithPermissionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RoleWithPermissionsDto> Handle(GetRoleWithPermissionsQuery request, CancellationToken cancellationToken)
    {
        var role = await _context.GetDbSet<AppRole>()
            .AsNoTracking()
            .Include(x => x.RolePermissions)
            .FirstOrDefaultAsync(x => x.Id == request.RoleId, cancellationToken);

        if (role == null) throw new NotFoundException(nameof(AppRole), request.RoleId);

        return new RoleWithPermissionsDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            PermissionIds = role.RolePermissions.Select(rp => rp.PermissionId).ToList()
        };
    }
}
