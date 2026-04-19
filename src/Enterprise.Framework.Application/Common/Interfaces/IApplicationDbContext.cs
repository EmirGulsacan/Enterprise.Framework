namespace Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Domain.Common;

using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Storage;



public interface IApplicationDbContext {

    DbSet<T> GetDbSet<T>() where T : class, IEntity;

    DbSet<Enterprise.Framework.Domain.Entities.Employee> Employees { get; }
    DbSet<Enterprise.Framework.Domain.Entities.Asset> Assets { get; }
    DbSet<Enterprise.Framework.Domain.Entities.Maintenance> Maintenances { get; }
    DbSet<Enterprise.Framework.Domain.Entities.Labor> Labors { get; }
    DbSet<Enterprise.Framework.Domain.Entities.Document> Documents { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    IExecutionStrategy CreateExecutionStrategy();
}



