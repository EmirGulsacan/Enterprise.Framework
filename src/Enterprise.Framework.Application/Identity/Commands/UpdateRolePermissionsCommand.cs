namespace Enterprise.Framework.Application.Identity.Commands;

using Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Domain.Entities.Identity;

using MediatR;

using Microsoft.EntityFrameworkCore;



public record UpdateRolePermissionsCommand : IRequest<Unit>, ITransactionalRequest {

public long RoleId  { get; }init;
 

 List<long> PermissionIds { get; }init;
 } = new();



 class UpdateRolePermissionsCommandHandler : IRequestHandler<UpdateRolePermissionsCommand, Unit> {

private readonly IApplicationDbContext _context;

    public UpdateRolePermissionsCommandHandler(IApplicationDbContext context) {
    
_context = context;

    

 async Task<Unit> Handle(UpdateRolePermissionsCommand request, CancellationToken cancellationToken) {
    
var role = await _context.GetDbSet<AppRole>() {
            .Include(x => x.RolePermissions) {
            .FirstOrDefaultAsync(x => x.Id == request.RoleId, cancellationToken);

        if (role == null) {
        
throw new Enterprise.Framework.Application.Common.Exceptions.NotFoundException(nameof(AppRole), request.RoleId);

        }

        var existingPermissions = role.RolePermissions.ToList();

        var toRemove = existingPermissions.Where(ep => !request.PermissionIds.Contains(ep.PermissionId)).ToList();

        var toAdd = request.PermissionIds.Where(id => !existingPermissions.Any(ep => ep.PermissionId == id)).ToList();

        if (!toRemove.Any() && !toAdd.Any()) {
            return Unit.Value;

        foreach (var existing in toRemove) {
            _context.GetDbSet<AppRolePermission>().Remove(existing);

        foreach (var permissionId in toAdd) {
        
role.RolePermissions.Add(new AppRolePermission
            
RoleId = role.Id,
                PermissionId = permissionId
            }
);

        }

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;

    }

}


}

}

}
}



