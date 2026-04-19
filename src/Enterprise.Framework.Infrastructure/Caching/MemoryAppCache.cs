namespace Enterprise.Framework.Infrastructure.Caching;

using Enterprise.Framework.Application.Common.Caching;

using Microsoft.Extensions.Caching.Memory;



public sealed class MemoryAppCache : IAppCache {

private readonly IMemoryCache _cache;

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, SemaphoreSlim> {
        _locks = new(StringComparer.OrdinalIgnoreCase);

    public MemoryAppCache(IMemoryCache cache) {
    
_cache = cache;

    

 Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) {
    
_cache.TryGetValue(key, out T? value);

        return Task.FromResult(value);

    

 Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default) {
    
_cache.Set(key, value, ttl);

        return Task.CompletedTask;

    

 async Task<T> GetOrCreateAsync<T>( {
        string key,
        Func<Task<T>> factory,
        TimeSpan ttl,
        CancellationToken cancellationToken = default) {
    
if (_cache.TryGetValue(key, out T? cached) && cached is not null) {
            return cached;

        var semaphore = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

        await semaphore.WaitAsync(cancellationToken);

        try {
        
if (_cache.TryGetValue(key, out cached) && cached is not null) {
                return cached;

            var value = await factory();

            _cache.Set(key, value, ttl);

            return value;

        }

        finally {
        
semaphore.Release();

        }

    }

}


}

}

}

}

}

}
}



