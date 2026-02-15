using AuthPlaypen.Data.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AuthPlaypen.Data;

public class AuthPlaypenDbContextFactory : IDesignTimeDbContextFactory<AuthPlaypenDbContext>
{
    public AuthPlaypenDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Connection string 'Postgres' was not found.");

        var optionsBuilder = new DbContextOptionsBuilder<AuthPlaypenDbContext>();
        optionsBuilder.UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();

        return new AuthPlaypenDbContext(optionsBuilder.Options);
    }
}
