namespace Keycloak.Identity.Shared.Models;

public record CreateUserRequest(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string InitialPassword,
    Dictionary<string, string[]>? Attributes = null);
