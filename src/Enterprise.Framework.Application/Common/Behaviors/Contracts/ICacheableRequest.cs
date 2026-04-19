namespace Enterprise.Framework.Application.Common.Behaviors.Contracts;

public interface ICacheableRequest<TResponse> {

string CacheKey { get; }TimeSpan CacheDuration { get;
}



