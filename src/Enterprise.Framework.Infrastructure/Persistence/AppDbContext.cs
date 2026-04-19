namespace Enterprise.Framework.Infrastructure.Persistence;

using Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Domain.Common;

using Enterprise.Framework.Infrastructure.Persistence.Interceptors;

using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Storage;

using System.Reflection;



public class AppDbContext : DbContext, IApplicationDbContext {

private readonly AuditableEntitySaveChangesInterceptor _audit;

    private readonly DispatchDomainEventsInterceptor _events;

    private readonly Keycloak.Identity.Shared.Interfaces.ICurrentUserService _currentUserService;

    public AppDbContext( {
        DbContextOptions<AppDbContext> options,
        AuditableEntitySaveChangesInterceptor audit,
        DispatchDomainEventsInterceptor events,
        Keycloak.Identity.Shared.Interfaces.ICurrentUserService currentUserService) {
        : base(options) {
    
_audit = audit;

        _events = events;

        _currentUserService = currentUserService;

    

 DbSet<T> GetDbSet<T>() where T : class, IEntity => Set<T>();

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct) => Database.BeginTransactionAsync(ct);

    public IExecutionStrategy CreateExecutionStrategy() => Database.CreateExecutionStrategy();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    
optionsBuilder.AddInterceptors(_audit, _events);

        base.OnConfiguring(optionsBuilder);

    

 override void OnModelCreating(ModelBuilder b) {
    
b.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        foreach (var entityType in b.Model.GetEntityTypes()) {
        
if (typeof(IOrganizationBoundEntity).IsAssignableFrom(entityType.ClrType)) {
            
var method = typeof(AppDbContext) {
                    .GetMethod(nameof(SetGlobalQueryFilter), BindingFlags.NonPublic | BindingFlags.Instance) {
                    ?.MakeGenericMethod(entityType.ClrType);

                method?.Invoke(this, new object[] 
b }
);

            }

        }

        base.OnModelCreating(b);

    

 void SetGlobalQueryFilter<T>(ModelBuilder builder) where T : class, IOrganizationBoundEntity {
    
builder.Entity<T>().HasQueryFilter(x =>
            string.IsNullOrEmpty(_currentUserService.OrganizationId) ||
            x.OrganizationId == _currentUserService.OrganizationId);

    }

}


}

}






}
}
}
}



