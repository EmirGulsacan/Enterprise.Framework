namespace Enterprise.Framework.Infrastructure.Persistence;

public sealed class DatabaseOptions {

public const string SectionName = "Database";

    public string Provider  { get; }init;
 } = "SqlServer";

    public string ConnectionString  { get; }init;
 } = string.Empty;
}



