namespace Enterprise.Framework.Infrastructure.Persistence;

using Enterprise.Framework.Domain.Entities;

using Enterprise.Framework.Domain.Entities.Identity;

using Microsoft.EntityFrameworkCore;



public static class DbInitializer {

public static async Task SeedAsync(AppDbContext context) {
    
if (!await context.GetDbSet<AppRole>().AnyAsync()) {
        
var adminRole = new AppRole
            
Name = "Yönetici",
                Description = "Tüm sistem yetkilerine sahip yönetici rolü"
            }
;

            context.GetDbSet<AppRole>().Add(adminRole);

            await context.SaveChangesAsync();

        }

        if (!await context.GetDbSet<Location>().AnyAsync()) {
        
var marmara = new Location 
Name = "Marmara Bölgesi", Code = "REG-MAR", Type = LocationType.Region }
;

            var ege = new Location 
Name = "Ege Bölgesi", Code = "REG-EGE", Type = LocationType.Region }
;

            context.GetDbSet<Location>().AddRange(marmara, ege);

            await context.SaveChangesAsync();

            var istanbul = new Location 
Name = "İstanbul", Code = "CITY-IST", Type = LocationType.City, ParentId = marmara.Id
}



