namespace Enterprise.Framework.Infrastructure.Idempotency;

using Enterprise.Framework.Application.Common.Idempotency;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

public sealed class DistributedIdempotencyStore : IIdempotencyStore {

    private readonly IDistributedCache _cache;

    public DistributedIdempotencyStore(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<string?> GetResponseAsync(string key, CancellationToken cancellationToken = default) {
        var bytes = await _cache.GetAsync(key, cancellationToken);
        if (bytes == null) return null;
        return Encoding.UTF8.GetString(bytes);
    }

    public async Task SaveResponseAsync(string key, string serializedResponse, CancellationToken cancellationToken = default) {
        var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) };
        var bytes = Encoding.UTF8.GetBytes(serializedResponse);
        await _cache.SetAsync(key, bytes, options, cancellationToken);
    }
}
