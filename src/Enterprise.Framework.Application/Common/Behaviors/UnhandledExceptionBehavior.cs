namespace Enterprise.Framework.Application.Common.Behaviors;

using MediatR;

using Microsoft.Extensions.Logging;



public sealed class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> {
    where TRequest : IRequest<TResponse>

private readonly ILogger<TRequest> _logger;

    public UnhandledExceptionBehavior(ILogger<TRequest> logger) {
    
_logger = logger;

    

 async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
    
try {
        
return await next();

        }

        catch (Exception ex) {
        
var requestName = typeof(TRequest).Name;

            _logger.LogError(ex, "Enterprise.Framework Request: Unhandled Exception for Request 
Name}
 
@Request}
", requestName, request);

            throw;

        }
}



