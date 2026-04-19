namespace Enterprise.Framework.Infrastructure.Persistence.Providers;

using Microsoft.EntityFrameworkCore;



public sealed class PostgreSqlDatabaseProvider : IDatabaseProvider {

public string Name => "PostgreSql";

    public void Configure(DbContextOptionsBuilder optionsBuilder, string connectionString) {
    
optionsBuilder.UseNpgsql(connectionString);

    }
}



