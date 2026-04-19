namespace Enterprise.Framework.Application.Common.Idempotency;

public interface IIdempotencyStore {

Task<string?> GetResponseAsync(string key, CancellationToken cancellationToken = default);

    Task SaveResponseAsync(string key, string serializedResponse, CancellationToken cancellationToken = default);
}



