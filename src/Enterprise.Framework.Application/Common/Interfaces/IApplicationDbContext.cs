namespace Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Domain.Common;

using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Storage;



public interface IApplicationDbContext {

DbSet<T> GetDbSet<T>() where T : class, IEntity;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    IExecutionStrategy CreateExecutionStrategy();
}



