namespace Enterprise.Framework.Application.Features.Identity.Commands;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public sealed record DeleteRoleCommand(long Id) : IRequest<Unit>;

class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IKeycloakAdminService _keycloak;
    private readonly ILogger<DeleteRoleCommandHandler> _logger;

    public DeleteRoleCommandHandler(
        IApplicationDbContext context,
        IKeycloakAdminService keycloak,
        ILogger<DeleteRoleCommandHandler> logger)
    {
        _context = context;
        _keycloak = keycloak;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _context.GetDbSet<AppRole>()
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (role == null)
            throw new InvalidOperationException($"Role with ID {request.Id} not found.");

        _context.GetDbSet<AppRole>().Remove(role);
        await _context.SaveChangesAsync(cancellationToken);

        try
        {
            await _keycloak.DeleteRoleAsync(role.Name, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex,
                "Role '{RoleName}' deleted locally but Keycloak sync failed.",
                role.Name);
        }

        return Unit.Value;
    }
}
