namespace Enterprise.Framework.Application.Common.Behaviors;

using Enterprise.Framework.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public sealed class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(IApplicationDbContext dbContext, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ITransactionalRequest)
        {
            return await next();
        }

        var strategy = _dbContext.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.BeginTransactionAsync(cancellationToken);
            try
            {
                _logger.LogInformation("Beginning transaction for {RequestName}", typeof(TRequest).Name);
                var response = await next();
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                _logger.LogInformation("Committed transaction for {RequestName}", typeof(TRequest).Name);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rolling back transaction for {RequestName}", typeof(TRequest).Name);
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }
}
