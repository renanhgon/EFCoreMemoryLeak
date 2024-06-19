using Microsoft.Extensions.Configuration;

namespace Config.Database;

public class DatabaseConfig(IConfiguration configuration) : IDatabaseConfig
{
    public string ConnectionString
        => configuration.GetConnectionString("MyDbContext")!;
}