using MarketingAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace MarketingAPI;

/// <summary>
/// Extension methods for configuring the database context.
/// </summary>
public static class DatabaseServiceExtension
{
    /// <summary>
    /// Adds the database context to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");
        string? provider = configuration["DatabaseProvider"];

        switch (provider)
        {
#if DEBUG
            case "SQLite":
                services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
                break;
#endif
            case "MySQL":
                services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
                break;

            default:
                throw new InvalidOperationException($"Unsupported database provider: {provider}");
        }

        return services;
    }
}