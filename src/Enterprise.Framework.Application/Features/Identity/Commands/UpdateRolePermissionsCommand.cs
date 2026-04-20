namespace Enterprise.Framework.Application.Features.Identity.Commands;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Enterprise.Framework.Application.Common.Exceptions;

public sealed record UpdateRolePermissionsCommand : IRequest<Unit>
{
    public long RoleId { get; init; }
    public List<long> PermissionIds { get; init; } = new();
}

class UpdateRolePermissionsCommandHandler : IRequestHandler<UpdateRolePermissionsCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateRolePermissionsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        var role = await _context.GetDbSet<AppRole>()
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);

        if (role == null)
        {
            throw new NotFoundException(nameof(AppRole), request.RoleId);
        }

        var existingPermissions = role.RolePermissions.ToList();
        var toRemove = existingPermissions.Where(ep => !request.PermissionIds.Contains(ep.PermissionId)).ToList();
        var toAdd = request.PermissionIds.Where(id => !existingPermissions.Any(ep => ep.PermissionId == id)).ToList();

        if (!toRemove.Any() && !toAdd.Any())
        {
            return Unit.Value;
        }

        foreach (var existing in toRemove)
        {
            _context.GetDbSet<AppRolePermission>().Remove(existing);
        }

        foreach (var permissionId in toAdd)
        {
            role.RolePermissions.Add(new AppRolePermission
            {
                RoleId = role.Id,
                PermissionId = permissionId
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
