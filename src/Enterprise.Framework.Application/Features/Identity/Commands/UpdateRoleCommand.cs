namespace Enterprise.Framework.Application.Features.Identity.Commands;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public sealed record UpdateRoleCommand : IRequest<Unit>
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IKeycloakAdminService _keycloak;
    private readonly ILogger<UpdateRoleCommandHandler> _logger;

    public UpdateRoleCommandHandler(
        IApplicationDbContext context,
        IKeycloakAdminService keycloak,
        ILogger<UpdateRoleCommandHandler> logger)
    {
        _context = context;
        _keycloak = keycloak;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _context.GetDbSet<AppRole>()
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (role == null)
            throw new InvalidOperationException($"Role with ID {request.Id} not found.");

        var oldName = role.Name;

        role.Name = request.Name;
        role.Description = request.Description;

        await _context.SaveChangesAsync(cancellationToken);

        try
        {
            await _keycloak.UpdateRoleAsync(oldName, role.Name, role.Description, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex,
                "Role '{RoleName}' updated locally but Keycloak sync failed.",
                role.Name);
        }

        return Unit.Value;
    }
}
