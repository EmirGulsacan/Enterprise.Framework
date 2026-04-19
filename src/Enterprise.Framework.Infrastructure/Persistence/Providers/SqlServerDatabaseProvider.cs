namespace Enterprise.Framework.Infrastructure.Persistence.Providers;

using Microsoft.EntityFrameworkCore;



public sealed class SqlServerDatabaseProvider : IDatabaseProvider {

public string Name => "SqlServer";

    public void Configure(DbContextOptionsBuilder optionsBuilder, string connectionString) {
    
optionsBuilder.UseSqlServer(connectionString);

    }
}



