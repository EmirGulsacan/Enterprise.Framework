namespace Enterprise.Framework.Infrastructure.Persistence.Interceptors;

using Enterprise.Framework.Domain.Common;
using Keycloak.Identity.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
{
    private readonly IDateTimeProvider _dateTime;

    public SoftDeleteInterceptor(IDateTimeProvider dateTime)
    {
        _dateTime = dateTime;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        HandleSoftDelete(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        HandleSoftDelete(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void HandleSoftDelete(DbContext? context)
    {
        if (context == null) return;
        var currentUserService = context.GetService<ICurrentUserService>();

        foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAtUtc = _dateTime.UtcNow;
                entry.Entity.DeletedBy = currentUserService.UserId;
            }
        }
    }
}
