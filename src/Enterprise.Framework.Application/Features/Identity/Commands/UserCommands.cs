namespace Enterprise.Framework.Application.Features.Identity.Commands;

using Enterprise.Framework.Application.Common.Exceptions;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed record SyncUserCommand(string ExternalId, string Username, string Email, string FirstName, string LastName) : IRequest<long>;

public sealed record UpdateUserRolesCommand(long UserId, List<long> RoleIds) : IRequest<Unit>;

public sealed record CreateUserCommand(string Username, string Email, string FirstName, string LastName, string Password) : IRequest<long>;

public sealed record UpdateUserCommand(long Id, string Email, string FirstName, string LastName, bool IsActive) : IRequest<Unit>;

public sealed record DeleteUserCommand(long Id) : IRequest<Unit>;

sealed class UserCommandHandler : 
    IRequestHandler<SyncUserCommand, long>,
    IRequestHandler<UpdateUserRolesCommand, Unit>,
    IRequestHandler<CreateUserCommand, long>,
    IRequestHandler<UpdateUserCommand, Unit>,
    IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IKeycloakAdminService _keycloak;

    public UserCommandHandler(IApplicationDbContext context, IKeycloakAdminService keycloak)
    {
        _context = context;
        _keycloak = keycloak;
    }

    public async Task<long> Handle(SyncUserCommand req, CancellationToken ct)
    {
        var user = await _context.GetDbSet<AppUser>().FirstOrDefaultAsync(u => u.IdentityId == req.ExternalId, ct);

        if (user == null)
        {
            user = new AppUser
            {
                IdentityId = req.ExternalId,
                Username = req.Username,
                Email = req.Email,
                FirstName = req.FirstName,
                LastName = req.LastName
            };
            _context.GetDbSet<AppUser>().Add(user);
        }
        else
        {
            user.Username = req.Username;
            user.Email = req.Email;
            user.FirstName = req.FirstName;
            user.LastName = req.LastName;
        }

        await _context.SaveChangesAsync(ct);
        return user.Id;
    }

    public async Task<Unit> Handle(UpdateUserRolesCommand req, CancellationToken ct)
    {
        var user = await _context.GetDbSet<AppUser>().FirstOrDefaultAsync(u => u.Id == req.UserId, ct);
        if (user == null) throw new Exception("User not found");

        var currentRoles = await _keycloak.GetUserRoleNamesAsync(user.IdentityId, ct);
        var currentSet = currentRoles.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var desiredRoles = await _context.GetDbSet<AppRole>()
            .Where(r => req.RoleIds.Contains(r.Id))
            .Select(r => r.Name)
            .ToListAsync(ct);

        var desiredSet = desiredRoles.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var toAdd = desiredSet.Except(currentSet).ToList();
        var toRemove = currentSet.Except(desiredSet).ToList();

        if (toAdd.Any())
        {
            await _keycloak.AssignRolesToUserAsync(user.IdentityId, toAdd, ct);
        }

        if (toRemove.Any())
        {
            await _keycloak.RemoveRolesFromUserAsync(user.IdentityId, toRemove, ct);
        }

        var existing = _context.GetDbSet<AppUserRole>().Where(ur => ur.UserId == user.Id);
        _context.GetDbSet<AppUserRole>().RemoveRange(existing);

        foreach (var roleId in req.RoleIds)
        {
            _context.GetDbSet<AppUserRole>().Add(new AppUserRole { UserId = user.Id, RoleId = roleId });
        }

        await _context.SaveChangesAsync(ct);
        return Unit.Value;
    }

    public async Task<long> Handle(CreateUserCommand req, CancellationToken ct)
    {
        var identityId = await _keycloak.CreateUserAsync(req.Username, req.Email, req.FirstName, req.LastName, req.Password, ct);

        var user = new AppUser
        {
            IdentityId = identityId,
            Username = req.Username,
            Email = req.Email,
            FirstName = req.FirstName,
            LastName = req.LastName
        };

        _context.GetDbSet<AppUser>().Add(user);
        await _context.SaveChangesAsync(ct);

        return user.Id;
    }

    public async Task<Unit> Handle(UpdateUserCommand req, CancellationToken ct)
    {
        var user = await _context.GetDbSet<AppUser>().FirstOrDefaultAsync(u => u.Id == req.Id, ct);
        if (user == null) throw new NotFoundException(nameof(AppUser), req.Id);

        await _keycloak.UpdateUserAsync(user.IdentityId, req.Email, req.FirstName, req.LastName, req.IsActive, ct);

        user.Email = req.Email;
        user.FirstName = req.FirstName;
        user.LastName = req.LastName;
        user.IsActive = req.IsActive;

        await _context.SaveChangesAsync(ct);
        return Unit.Value;
    }

    public async Task<Unit> Handle(DeleteUserCommand req, CancellationToken ct)
    {
        var user = await _context.GetDbSet<AppUser>().FirstOrDefaultAsync(u => u.Id == req.Id, ct);
        if (user == null) throw new NotFoundException(nameof(AppUser), req.Id);

        await _keycloak.DeleteUserAsync(user.IdentityId, ct);

        _context.GetDbSet<AppUser>().Remove(user);
        await _context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
