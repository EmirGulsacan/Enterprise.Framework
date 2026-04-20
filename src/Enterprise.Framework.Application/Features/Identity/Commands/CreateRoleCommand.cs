namespace Enterprise.Framework.Application.Features.Identity.Commands;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public sealed record CreateRoleCommand : IRequest<long>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, long>
{
    private readonly IApplicationDbContext _context;
    private readonly IKeycloakAdminService _keycloak;
    private readonly ILogger<CreateRoleCommandHandler> _logger;

    public CreateRoleCommandHandler(
        IApplicationDbContext context,
        IKeycloakAdminService keycloak,
        ILogger<CreateRoleCommandHandler> logger)
    {
        _context = context;
        _keycloak = keycloak;
        _logger = logger;
    }

    public async Task<long> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = new AppRole
        {
            Name = request.Name,
            Description = request.Description
        };

        _context.GetDbSet<AppRole>().Add(role);
        await _context.SaveChangesAsync(cancellationToken);

        try
        {
            await _keycloak.CreateRoleAsync(role.Name, role.Description, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex,
                "Role '{RoleName}' saved locally but Keycloak sync failed. " +
                "JWT tokens will NOT contain this role until Keycloak is synced.",
                role.Name);
        }

        return role.Id;
    }
}
