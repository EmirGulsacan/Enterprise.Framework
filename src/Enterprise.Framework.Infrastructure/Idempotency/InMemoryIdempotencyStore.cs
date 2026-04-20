namespace Enterprise.Framework.Infrastructure.Idempotency;

using Enterprise.Framework.Application.Common.Idempotency;
using Microsoft.Extensions.Caching.Memory;

public sealed class InMemoryIdempotencyStore : IIdempotencyStore {

    private readonly IMemoryCache _cache;

    public InMemoryIdempotencyStore(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<string?> GetResponseAsync(string key, CancellationToken cancellationToken = default) {
        _cache.TryGetValue(key, out string? response);
        return Task.FromResult(response);
    }

    public Task SaveResponseAsync(string key, string serializedResponse, CancellationToken cancellationToken = default) {
        var options = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(24));
        _cache.Set(key, serializedResponse, options);
        return Task.CompletedTask;
    }
}
