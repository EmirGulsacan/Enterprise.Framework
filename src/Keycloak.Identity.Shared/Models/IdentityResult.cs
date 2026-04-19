namespace Keycloak.Identity.Shared.Models;

public class IdentityResult
{
    public bool Succeeded { get; private set; }
    public string[] Errors { get; private set; }
    public string? ExternalId { get; private set; }

    private IdentityResult(bool succeeded, IEnumerable<string> errors, string? externalId = null)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
        ExternalId = externalId;
    }

    public static IdentityResult Success(string? externalId = null)
    {
        return new IdentityResult(true, Array.Empty<string>(), externalId);
    }

    public static IdentityResult Failure(IEnumerable<string> errors)
    {
        return new IdentityResult(false, errors);
    }
}
