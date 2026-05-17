namespace Enterprise.Framework.Infrastructure.Persistence;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;
using Enterprise.Framework.Domain.Entities;
using Enterprise.Framework.Domain.Rules;

public class AppDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;
    }


    public DbSet<T> GetDbSet<T>() where T : class, IEntity => Set<T>();

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct) => 
        Database.BeginTransactionAsync(ct);

    public IExecutionStrategy CreateExecutionStrategy() => Database.CreateExecutionStrategy();


    protected override void OnModelCreating(ModelBuilder b)
    {
        b.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        foreach (var entityType in b.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType.IsEnum)
                {
                    property.SetProviderClrType(typeof(string));
                    property.SetMaxLength(50);
                }
            }

            if (typeof(IOrganizationBoundEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(AppDbContext)
                    .GetMethod(nameof(SetOrganizationQueryFilter), BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.MakeGenericMethod(entityType.ClrType);

                method?.Invoke(this, new object[] { b });
            }

            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(AppDbContext)
                    .GetMethod(nameof(SetSoftDeleteQueryFilter), BindingFlags.NonPublic | BindingFlags.Static)
                    ?.MakeGenericMethod(entityType.ClrType);

                method?.Invoke(null, new object[] { b });
            }
        }

        base.OnModelCreating(b);
    }

    private void SetOrganizationQueryFilter<T>(ModelBuilder builder) where T : class, IOrganizationBoundEntity
    {
        builder.Entity<T>().HasQueryFilter(x =>
            string.IsNullOrEmpty(_currentUserService.OrganizationId) ||
            x.OrganizationId == _currentUserService.OrganizationId);
    }

    private static void SetSoftDeleteQueryFilter<T>(ModelBuilder builder) where T : class, ISoftDeletable
    {
        builder.Entity<T>().HasQueryFilter(x => !x.IsDeleted);
    }
}

