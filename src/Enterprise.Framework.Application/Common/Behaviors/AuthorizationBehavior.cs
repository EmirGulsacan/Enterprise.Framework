namespace Enterprise.Framework.Application.Common.Behaviors;

using Enterprise.Framework.Application.Common.Behaviors.Contracts;
using Keycloak.Identity.Shared.Interfaces;
using MediatR;

public sealed class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUser;

    public AuthorizationBehavior(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IAuthorizableRequest secured || secured.RequiredPermissions.Count == 0 || _currentUser.IsAdmin)
        {
            return await next();
        }

        if (!_currentUser.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("Kimlik dogrulamasi gerekli.");
        }

        var permissionTasks = secured.RequiredPermissions
            .Select(p => _currentUser.HasPermissionAsync(p, cancellationToken))
            .ToList();

        var results = await Task.WhenAll(permissionTasks);

        var missing = secured.RequiredPermissions
            .Zip(results, (permission, hasIt) => (permission, hasIt))
            .Where(x => !x.hasIt)
            .Select(x => x.permission)
            .ToList();

        if (missing.Count != 0)
        {
            throw new UnauthorizedAccessException($"Yetki yetersiz. Eksik izinler: {string.Join(", ", missing)}");
        }

        return await next();
    }
}
