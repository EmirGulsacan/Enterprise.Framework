namespace Enterprise.Framework.Application.Identity.Commands;

using FluentValidation;

using Enterprise.Framework.Application.Common.Exceptions;

using Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Domain.Entities.Identity;

using MediatR;

using Microsoft.EntityFrameworkCore;



public record AssignPermissionToRoleCommand(long RoleId, long PermissionId) : IRequest;

public class AssignPermissionToRoleValidator : FluentValidation.AbstractValidator<AssignPermissionToRoleCommand> {

public AssignPermissionToRoleValidator() {
    
RuleFor(x => x.RoleId).GreaterThan(0);

        RuleFor(x => x.PermissionId).GreaterThan(0);

    }



 class AssignPermissionToRoleCommandHandler : IRequestHandler<AssignPermissionToRoleCommand> {

private readonly IApplicationDbContext _context;

    public AssignPermissionToRoleCommandHandler(IApplicationDbContext context) {
    
_context = context;

    

 async Task<Unit> Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken) {
    
var result = await _context.GetDbSet<AppRole>() {
            .AsNoTracking() {
            .Where(r => r.Id == request.RoleId) {
            .Select(r => new
            
RoleExists = true,
                AlreadyAssigned = r.RolePermissions.Any(rp => rp.PermissionId == request.PermissionId),
                PermissionExists = _context.GetDbSet<AppPermission>().Any(p => p.Id == request.PermissionId) {
            }
) {
            .FirstOrDefaultAsync(cancellationToken);

        if (result == null) throw new NotFoundException(nameof(AppRole), request.RoleId);

        if (!result.PermissionExists) throw new NotFoundException(nameof(AppPermission), request.PermissionId);

        if (!result.AlreadyAssigned) {
        
_context.GetDbSet<AppRolePermission>().Add(new AppRolePermission
            
RoleId = request.RoleId,
                PermissionId = request.PermissionId
            }
);

            await _context.SaveChangesAsync(cancellationToken);

        }

        return Unit.Value;

    }

}







}
}
}
}
}



