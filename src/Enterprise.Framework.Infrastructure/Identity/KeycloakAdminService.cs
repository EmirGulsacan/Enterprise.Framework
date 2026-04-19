using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Enterprise.Framework.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Enterprise.Framework.Infrastructure.Identity;

public class KeycloakAdminService : IKeycloakAdminService
{
    private const string TokenCacheKey = "keycloak:admin:token";

    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<KeycloakAdminService> _logger;
    private readonly IMemoryCache _cache;

    private static readonly SemaphoreSlim _tokenSemaphore = new(1, 1);

    public KeycloakAdminService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<KeycloakAdminService> logger,
        IMemoryCache cache)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _cache = cache;
    }

    private async Task<string> GetAdminTokenAsync(CancellationToken ct)
    {
        if (_cache.TryGetValue(TokenCacheKey, out string? cachedToken) && cachedToken != null)
            return cachedToken;

        await _tokenSemaphore.WaitAsync(ct);
        try
        {
            if (_cache.TryGetValue(TokenCacheKey, out cachedToken) && cachedToken != null)
                return cachedToken;

            return await FetchAndCacheTokenAsync(ct);
        }
        finally
        {
            _tokenSemaphore.Release();
        }
    }

    private async Task<string> FetchAndCacheTokenAsync(CancellationToken ct)
    {
        var authServerUrl = _configuration["Keycloak:auth-server-url"]?.TrimEnd('/');
        var realm = _configuration["Keycloak:realm"]?.Trim();
        var clientId = _configuration["Keycloak:AdminClientId"]?.Trim();
        var clientSecret = _configuration["Keycloak:AdminClientSecret"]?.Trim();

        if (string.IsNullOrEmpty(authServerUrl) || string.IsNullOrEmpty(realm)
            || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            _logger.LogCritical("Keycloak configuration is incomplete. Url:{Url} Realm:{Realm} ClientId:{ClientId}",
                authServerUrl, realm, clientId);
            throw new InvalidOperationException("Keycloak configuration is missing or empty.");
        }

        var tokenUrl = $"{authServerUrl}/realms/{realm}/protocol/openid-connect/token";
        _logger.LogInformation("Fetching Keycloak admin token from {Url}", tokenUrl);

        var payload = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", clientId },
            { "client_secret", clientSecret }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl)
        {
            Content = new FormUrlEncodedContent(payload)
        };

        var response = await _httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("Token fetch failed. Status:{Status} Error:{Error}", response.StatusCode, error);
            throw new InvalidOperationException($"Keycloak auth failed. Status:{response.StatusCode} Details:{error}");
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
        var token = json.GetProperty("access_token").GetString()
            ?? throw new InvalidOperationException("access_token not found in Keycloak response.");

        var expiresIn = json.TryGetProperty("expires_in", out var expiresEl)
            ? expiresEl.GetInt32()
            : 300;

        _cache.Set(TokenCacheKey, token,
            TimeSpan.FromSeconds(Math.Max(expiresIn - 30, 10)));

        return token;
    }

    private string GetBaseUrl() =>
        $"{_configuration["Keycloak:auth-server-url"]?.TrimEnd('/')}/admin/realms/{_configuration["Keycloak:realm"]}";

    private async Task<HttpRequestMessage> BuildRequestAsync(HttpMethod method, string url, object? body, CancellationToken ct)
    {
        var token = await GetAdminTokenAsync(ct);
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (body != null)
            request.Content = JsonContent.Create(body);
        return request;
    }

    public async Task<string> CreateUserAsync(string username, string email, string firstName,
        string lastName, string password, CancellationToken ct = default)
    {
        var url = $"{GetBaseUrl()}/users";
        var body = new
        {
            username, email, firstName, lastName, enabled = true,
            credentials = new[] { new { type = "password", value = password, temporary = false } }
        };

        var request = await BuildRequestAsync(HttpMethod.Post, url, body, ct);
        var response = await _httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("User creation failed: {Error}", error);
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                throw new InvalidOperationException("Username or email already exists in Keycloak.");
            throw new InvalidOperationException("Keycloak user creation failed.");
        }

        return response.Headers.Location?.Segments.Last()
            ?? throw new InvalidOperationException("User created but ID could not be retrieved.");
    }

    public async Task UpdateUserAsync(string identityId, string email, string firstName,
        string lastName, bool enabled, CancellationToken ct = default)
    {
        var url = $"{GetBaseUrl()}/users/{identityId}";
        var body = new { email, firstName, lastName, enabled };
        var request = await BuildRequestAsync(HttpMethod.Put, url, body, ct);
        var response = await _httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("User update failed for {Id}: {Error}", identityId, error);
            throw new InvalidOperationException("Keycloak user update failed.");
        }
    }

    public async Task DeleteUserAsync(string identityId, CancellationToken ct = default)
    {
        var url = $"{GetBaseUrl()}/users/{identityId}";
        var request = await BuildRequestAsync(HttpMethod.Delete, url, null, ct);
        var response = await _httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("User deletion failed for {Id}: {Error}", identityId, error);
            throw new InvalidOperationException("Keycloak user deletion failed.");
        }
    }

    public async Task ResetPasswordAsync(string identityId, string newPassword, CancellationToken ct = default)
    {
        var url = $"{GetBaseUrl()}/users/{identityId}/reset-password";
        var body = new { type = "password", value = newPassword, temporary = false };
        var request = await BuildRequestAsync(HttpMethod.Put, url, body, ct);
        var response = await _httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyList<string>> GetAllRealmRoleNamesAsync(CancellationToken ct = default)
    {
        var url = $"{GetBaseUrl()}/roles?max=1000";
        var request = await BuildRequestAsync(HttpMethod.Get, url, null, ct);
        var response = await _httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var roles = await response.Content.ReadFromJsonAsync<JsonElement[]>(cancellationToken: ct)
            ?? Array.Empty<JsonElement>();

        return roles
            .Where(r => r.TryGetProperty("name", out _))
            .Select(r => r.GetProperty("name").GetString()!)
            .ToList();
    }

    public async Task CreateRoleAsync(string roleName, string? description = null, CancellationToken ct = default)
    {
        var url = $"{GetBaseUrl()}/roles";
        var body = new { name = roleName, description };
        var json = JsonSerializer.Serialize(body);
        var token = await GetAdminTokenAsync(ct);

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.Conflict)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("Role creation failed for '{Role}': {Error}", roleName, error);
            throw new InvalidOperationException($"Keycloak role '{roleName}' could not be created. Details: {error}");
        }
    }

    public async Task<IReadOnlyList<string>> GetUserRoleNamesAsync(string identityId, CancellationToken ct = default)
    {
        var url = $"{GetBaseUrl()}/users/{identityId}/role-mappings/realm";
        var request = await BuildRequestAsync(HttpMethod.Get, url, null, ct);
        var response = await _httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var roles = await response.Content.ReadFromJsonAsync<JsonElement[]>(cancellationToken: ct)
            ?? Array.Empty<JsonElement>();

        return roles
            .Where(r => r.TryGetProperty("name", out _))
            .Select(r => r.GetProperty("name").GetString()!)
            .ToList();
    }

    public async Task AssignRolesToUserAsync(string identityId, IEnumerable<string> roleNames, CancellationToken ct = default)
    {
        var names = roleNames.ToList();
        if (!names.Any()) return;

        var roleObjects = await ResolveRoleObjectsAsync(names, ct);
        if (!roleObjects.Any()) return;

        var url = $"{GetBaseUrl()}/users/{identityId}/role-mappings/realm";
        var request = await BuildRequestAsync(HttpMethod.Post, url, roleObjects, ct);
        var response = await _httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("Bulk role assignment failed for {Id}: {Error}", identityId, error);
            throw new InvalidOperationException("Keycloak role assignment failed.");
        }
    }

    public async Task RemoveRolesFromUserAsync(string identityId, IEnumerable<string> roleNames, CancellationToken ct = default)
    {
        var names = roleNames.ToList();
        if (!names.Any()) return;

        var roleObjects = await ResolveRoleObjectsAsync(names, ct);
        if (!roleObjects.Any()) return;

        var url = $"{GetBaseUrl()}/users/{identityId}/role-mappings/realm";
        var request = await BuildRequestAsync(HttpMethod.Delete, url, roleObjects, ct);
        var response = await _httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("Bulk role removal failed for {Id}: {Error}", identityId, error);
            throw new InvalidOperationException("Keycloak role removal failed.");
        }
    }

    private async Task<List<object>> ResolveRoleObjectsAsync(IEnumerable<string> roleNames, CancellationToken ct)
    {
        var results = new List<object>();
        foreach (var roleName in roleNames)
        {
            var url = $"{GetBaseUrl()}/roles/{roleName}";
            var request = await BuildRequestAsync(HttpMethod.Get, url, null, ct);
            var response = await _httpClient.SendAsync(request, ct);

            if (response.IsSuccessStatusCode)
            {
                var role = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
                results.Add(role);
            }
            else
            {
                _logger.LogWarning("Keycloak role '{Role}' not found. Skipping.", roleName);
            }
        }
        return results;
    }
}

