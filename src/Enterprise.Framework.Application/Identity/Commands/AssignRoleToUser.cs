namespace Enterprise.Framework.Application.Identity.Commands;

using FluentValidation;

using Enterprise.Framework.Application.Common.Exceptions;

using Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Domain.Entities.Identity;

using MediatR;

using Microsoft.EntityFrameworkCore;



public record AssignRoleToUserCommand(long UserId, long RoleId) : IRequest;

public class AssignRoleToUserValidator : AbstractValidator<AssignRoleToUserCommand> {

public AssignRoleToUserValidator() {
    
RuleFor(x => x.UserId).GreaterThan(0);

        RuleFor(x => x.RoleId).GreaterThan(0);

    }



 class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand> {

private readonly IApplicationDbContext _context;

    public AssignRoleToUserCommandHandler(IApplicationDbContext context) {
    
_context = context;

    

 async Task<Unit> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken) {
    
var result = await _context.GetDbSet<AppUser>() {
            .AsNoTracking() {
            .Where(u => u.Id == request.UserId) {
            .Select(u => new
            
UserExists = true,
                AlreadyAssigned = u.UserRoles.Any(ur => ur.RoleId == request.RoleId),
                RoleExists = _context.GetDbSet<AppRole>().Any(r => r.Id == request.RoleId) {
            }
) {
            .FirstOrDefaultAsync(cancellationToken);

        if (result == null) throw new NotFoundException(nameof(AppUser), request.UserId);

        if (!result.RoleExists) throw new NotFoundException(nameof(AppRole), request.RoleId);

        if (!result.AlreadyAssigned) {
        
_context.GetDbSet<AppUserRole>().Add(new AppUserRole
            
UserId = request.UserId,
                RoleId = request.RoleId
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



