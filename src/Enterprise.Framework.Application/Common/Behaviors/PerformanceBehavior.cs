namespace Enterprise.Framework.Application.Common.Behaviors;

using MediatR;

using Microsoft.Extensions.Logging;

using System.Diagnostics;



public sealed class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> {
    where TRequest : IRequest<TResponse>

private const int SlowRequestThresholdMs = 500;

    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger) {
    
_logger = logger;

    

 async Task<TResponse> Handle( {
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken) {
    
var stopwatch = Stopwatch.StartNew();

        var response = await next();

        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds > SlowRequestThresholdMs) {
        
_logger.LogWarning(
                "Slow request detected 
RequestName}
. Elapsed: 
ElapsedMilliseconds}
ms",
                typeof(TRequest).Name,
                stopwatch.ElapsedMilliseconds);

        }

        return response;

    }
}



