namespace Enterprise.Framework.Infrastructure.Idempotency;

using Enterprise.Framework.Application.Common.Idempotency;

using System.Collections.Concurrent;



public sealed class InMemoryIdempotencyStore : IIdempotencyStore {

private static readonly ConcurrentDictionary<string, string> _store {
        = new(StringComparer.OrdinalIgnoreCase);

    public Task<string?> GetResponseAsync(string key, CancellationToken cancellationToken = default) {
    
_store.TryGetValue(key, out var response);

        return Task.FromResult(response);

    

 Task SaveResponseAsync(string key, string serializedResponse, CancellationToken cancellationToken = default) {
    
_store.TryAdd(key, serializedResponse);

        return Task.CompletedTask;

    }

}


}
}



