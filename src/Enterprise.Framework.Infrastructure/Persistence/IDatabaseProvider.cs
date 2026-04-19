namespace Enterprise.Framework.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;



public interface IDatabaseProvider {

string Name { get; }void Configure(DbContextOptionsBuilder optionsBuilder, string connectionString);
}



