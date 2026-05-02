namespace Enterprise.Framework.Infrastructure.Caching;

using Enterprise.Framework.Application.Common.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;
using System.Text.Json;

public sealed class DistributedAppCache : IAppCache {

    private readonly IDistributedCache _cache;
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new(StringComparer.OrdinalIgnoreCase);

    public DistributedAppCache(IDistributedCache cache) {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) {
        var valueBytes = await _cache.GetAsync(key, cancellationToken);
        if (valueBytes == null) return default;
        return JsonSerializer.Deserialize<T>(valueBytes);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default) {
        var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl };
        var valueBytes = JsonSerializer.SerializeToUtf8Bytes(value);
        await _cache.SetAsync(key, valueBytes, options, cancellationToken);
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan ttl,
        CancellationToken cancellationToken = default) {
        
        var cachedBytes = await _cache.GetAsync(key, cancellationToken);
        if (cachedBytes != null) {
            var cached = JsonSerializer.Deserialize<T>(cachedBytes);
            if (cached is not null) return cached;
        }

        var semaphore = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(cancellationToken);
        
        try {
            cachedBytes = await _cache.GetAsync(key, cancellationToken);
            if (cachedBytes != null) {
                var cached = JsonSerializer.Deserialize<T>(cachedBytes);
                if (cached is not null) return cached;
            }

            var value = await factory();
            
            var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl };
            var valueBytes = JsonSerializer.SerializeToUtf8Bytes(value);
            await _cache.SetAsync(key, valueBytes, options, cancellationToken);

            return value;
        }
        finally {
            semaphore.Release();
            // In a highly concurrent scenario, removing the lock right away might cause another thread to create a new one.
            // Leaving it is fine for a limited number of keys.
        }
    }
}
