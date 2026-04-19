namespace Enterprise.Framework.Infrastructure.Persistence;

using Enterprise.Framework.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (!await context.GetDbSet<AppRole>().AnyAsync())
        {
            var adminRole = new AppRole
            {
                Name = "Admin",
                Description = "System Administrator"
            };

            context.GetDbSet<AppRole>().Add(adminRole);
            await context.SaveChangesAsync();
        }
    }
}
