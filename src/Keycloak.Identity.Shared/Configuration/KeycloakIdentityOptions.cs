namespace Keycloak.Identity.Shared.Configuration;

public class KeycloakIdentityOptions
{
    public const string SectionName = "Keycloak";

    public string AuthServerUrl { get; set; } = string.Empty;
    public string Realm { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public string Credentials { get; set; } = string.Empty;
    public string AdminClientId { get; set; } = string.Empty;
    public string AdminClientSecret { get; set; } = string.Empty;
}
