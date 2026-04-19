namespace Enterprise.Framework.Application.Common.Behaviors;

using Enterprise.Framework.Application.Common.Behaviors.Contracts;
using Enterprise.Framework.Application.Common.Caching;
using MediatR;

public sealed class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IAppCache _cache;

    public CachingBehavior(IAppCache cache)
    {
        _cache = cache;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICacheableRequest<TResponse> cacheable)
        {
            return await next();
        }

        return await _cache.GetOrCreateAsync(
            cacheable.CacheKey,
            () => next(),
            cacheable.CacheDuration,
            cancellationToken);
    }
}
