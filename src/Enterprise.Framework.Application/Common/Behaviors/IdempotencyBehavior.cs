namespace Enterprise.Framework.Application.Common.Behaviors;

using Enterprise.Framework.Application.Common.Behaviors.Contracts;

using Enterprise.Framework.Application.Common.Idempotency;

using MediatR;

using System.Text.Json;



public sealed class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> {
    where TRequest : IRequest<TResponse>

private readonly IIdempotencyStore _store;

    public IdempotencyBehavior(IIdempotencyStore store) {
    
_store = store;

    

 async Task<TResponse> Handle( {
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken) {
    
if (request is not IIdempotentCommand<TResponse> idempotent) {
            return await next();

        var existingJson = await _store.GetResponseAsync(idempotent.IdempotencyKey, cancellationToken);

        if (existingJson is not null) {
        
var previous = JsonSerializer.Deserialize<TResponse>(existingJson);

            return previous!;

        }

        var response = await next();

        var json = JsonSerializer.Serialize(response);

        await _store.SaveResponseAsync(idempotent.IdempotencyKey, json, cancellationToken);

        return response;

    }

}


}

}
}



