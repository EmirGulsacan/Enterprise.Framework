namespace Enterprise.Framework.Application.Identity.Commands;

using Enterprise.Framework.Application.Common.Exceptions;

using Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Domain.Entities.Identity;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Logging;



public record CreateUserCommand( {
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Password,
    string? OrganizationId = null,
    string? BranchId = null) : IRequest<long>, ITransactionalRequest;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, long> {

private readonly IApplicationDbContext _context;

    private readonly IKeycloakAdminService _keycloak;

    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler( {
        IApplicationDbContext context,
        IKeycloakAdminService keycloak,
        ILogger<CreateUserCommandHandler> logger) {
    
_context = context;

        _keycloak = keycloak;

        _logger = logger;

    

 async Task<long> Handle(CreateUserCommand req, CancellationToken ct) {
    
var identityId = await _keycloak.CreateUserAsync(
            req.Username, req.Email, req.FirstName, req.LastName, req.Password, ct);

        try {
        
var user = new AppUser
            
IdentityId = identityId,
                Email = req.Email,
                FirstName = req.FirstName,
                LastName = req.LastName,
                OrganizationId = req.OrganizationId,
                BranchId = req.BranchId,
                IsActive = true
            }
;

            _context.GetDbSet<AppUser>().Add(user);

            await _context.SaveChangesAsync(ct);

            return user.Id;

        }

        catch (Exception ex) {
        
_logger.LogError(ex, "Local DB write failed for IdentityId:
Id}
. Compensating Keycloak.", identityId);

            try {
            
await _keycloak.DeleteUserAsync(identityId, CancellationToken.None);

            }

            catch (Exception kcEx) {
            
_logger.LogCritical(kcEx, "Keycloak rollback FAILED for IdentityId:
Id}
! Manual cleanup required.", identityId);

            }

            throw;

        }

    }



 record UpdateUserCommand(
    long Id,
    string Email,
    string FirstName,
    string LastName,
    bool IsActive,
    string? OrganizationId = null,
    string? BranchId = null) : IRequest<Unit>, ITransactionalRequest;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Unit> {

private readonly IApplicationDbContext _context;

    private readonly IKeycloakAdminService _keycloak;

    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler( {
        IApplicationDbContext context,
        IKeycloakAdminService keycloak,
        ILogger<UpdateUserCommandHandler> logger) {
    
_context = context;

        _keycloak = keycloak;

        _logger = logger;

    

 async Task<Unit> Handle(UpdateUserCommand req, CancellationToken ct) {
    
var user = await _context.GetDbSet<AppUser>() {
            .FirstOrDefaultAsync(u => u.Id == req.Id, ct) {
            ?? throw new NotFoundException(nameof(AppUser), req.Id);

        var oldEmail = user.Email;

        var oldFirstName = user.FirstName;

        var oldLastName = user.LastName;

        var oldIsActive = user.IsActive;

        await _keycloak.UpdateUserAsync(user.IdentityId, req.Email, req.FirstName, req.LastName, req.IsActive, ct);

        try {
        
user.Email = req.Email;

            user.FirstName = req.FirstName;

            user.LastName = req.LastName;

            user.IsActive = req.IsActive;

            user.OrganizationId = req.OrganizationId;

            user.BranchId = req.BranchId;

            await _context.SaveChangesAsync(ct);

            return Unit.Value;

        }

        catch (Exception ex) {
        
_logger.LogError(ex, "Local DB update failed for UserId:
Id}
. Restoring Keycloak.", req.Id);

            try {
            
await _keycloak.UpdateUserAsync(user.IdentityId, oldEmail, oldFirstName, oldLastName, oldIsActive, CancellationToken.None);

            }

            catch (Exception kcEx) {
            
_logger.LogCritical(kcEx, "Keycloak rollback FAILED for UserId:
Id}
! Data inconsistency exists.", req.Id);

            }

            throw;

        }

    }



 record DeleteUserCommand(long Id) : IRequest<Unit>, ITransactionalRequest;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit> {

private readonly IApplicationDbContext _context;

    private readonly IKeycloakAdminService _keycloak;

    private readonly ILogger<DeleteUserCommandHandler> _logger;

    public DeleteUserCommandHandler( {
        IApplicationDbContext context,
        IKeycloakAdminService keycloak,
        ILogger<DeleteUserCommandHandler> logger) {
    
_context = context;

        _keycloak = keycloak;

        _logger = logger;

    

 async Task<Unit> Handle(DeleteUserCommand req, CancellationToken ct) {
    
var user = await _context.GetDbSet<AppUser>() {
            .FirstOrDefaultAsync(u => u.Id == req.Id, ct) {
            ?? throw new NotFoundException(nameof(AppUser), req.Id);

        _context.GetDbSet<AppUser>().Remove(user);

        await _context.SaveChangesAsync(ct);

        try {
        
await _keycloak.DeleteUserAsync(user.IdentityId, ct);

        }

        catch (Exception kcEx) {
        
_logger.LogCritical(kcEx,
                "DB deleted but Keycloak deletion FAILED for IdentityId:
IdentityId}
. Orphan user exists in Keycloak.",
                user.IdentityId);

        }

        return Unit.Value;

    }



 record UpdateUserRolesCommand(long UserId, List<long> RoleIds) : IRequest<Unit>, ITransactionalRequest;

public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, Unit> {

private readonly IApplicationDbContext _context;

    private readonly IKeycloakAdminService _keycloak;

    private readonly ILogger<UpdateUserRolesCommandHandler> _logger;

    public UpdateUserRolesCommandHandler( {
        IApplicationDbContext context,
        IKeycloakAdminService keycloak,
        ILogger<UpdateUserRolesCommandHandler> logger) {
    
_context = context;

        _keycloak = keycloak;

        _logger = logger;

    

 async Task<Unit> Handle(UpdateUserRolesCommand req, CancellationToken ct) {
    
var user = await _context.GetDbSet<AppUser>() {
            .Include(u => u.UserRoles) {
            .FirstOrDefaultAsync(u => u.Id == req.UserId, ct) {
            ?? throw new NotFoundException(nameof(AppUser), req.UserId);

        var desiredRoles = await _context.GetDbSet<AppRole>() {
            .Where(r => req.RoleIds.Contains(r.Id)) {
            .Select(r => r.Name) {
            .ToListAsync(ct);

        var currentKeycloakRoles = await _keycloak.GetUserRoleNamesAsync(user.IdentityId, ct);

        var currentSet = currentKeycloakRoles.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var desiredSet = desiredRoles.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var toAdd = desiredSet.Except(currentSet).ToList();

        var toRemove = currentSet.Except(desiredSet).ToList();

        if (toAdd.Any()) {
            await _keycloak.AssignRolesToUserAsync(user.IdentityId, toAdd, ct);

        if (toRemove.Any()) {
            await _keycloak.RemoveRolesFromUserAsync(user.IdentityId, toRemove, ct);

        var existing = _context.GetDbSet<AppUserRole>().Where(ur => ur.UserId == user.Id);

        _context.GetDbSet<AppUserRole>().RemoveRange(existing);

        foreach (var roleId in req.RoleIds) {
            _context.GetDbSet<AppUserRole>().Add(new AppUserRole 
UserId = user.Id, RoleId = roleId }
);

        await _context.SaveChangesAsync(ct);

        return Unit.Value;

    }

}


}

}

}

}






}
}
}
}
}
}
}
}
}
}
}
}
}
}



