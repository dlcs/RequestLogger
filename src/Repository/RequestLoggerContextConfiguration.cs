using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Repository;

public static class RequestLoggerContextConfiguration
{
    private static readonly string ConnectionStringKey = "PostgreSQLConnection";
    private static readonly string RunMigrationsKey = "RunMigrations";
    
    /// <summary>
    /// Register and configure <see cref="RequestLoggerContext"/> 
    /// </summary>
    public static IServiceCollection AddRequestLoggerContext(this IServiceCollection services,
        IConfiguration configuration)
        => services
            .AddDbContext<RequestLoggerContext>(options => SetupOptions(configuration, options));
    private static void SetupOptions(IConfiguration configuration,
        DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseNpgsql(configuration.GetConnectionString(ConnectionStringKey) ?? string.Empty)
            .UseSnakeCaseNamingConvention();
    
    /// <summary>
    /// Run EF migrations if "RunMigrations" = true
    /// </summary>
    public static void TryRunMigrations(IConfiguration configuration)
    {
        if (configuration.GetValue(RunMigrationsKey, false))
        {
            using var context = new RequestLoggerContext(GetOptionsBuilder(configuration).Options);
            var pendingMigrations = context.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Count == 0)
            {
                Log.Information("No migrations to run");
                return;
            }

            Log.Information("Running migrations: {Migrations}", string.Join(",", pendingMigrations));
            context.Database.Migrate();
        }
    }
    
    /// <summary>
    /// Get a new instantiated <see cref="RequestLoggerContext"/> object
    /// </summary>
    public static RequestLoggerContext GetNewDbContext(IConfiguration configuration)
        => new(GetOptionsBuilder(configuration).Options);

    private static DbContextOptionsBuilder<RequestLoggerContext> GetOptionsBuilder(IConfiguration configuration)
    {
        var optionsBuilder = new DbContextOptionsBuilder<RequestLoggerContext>();
        SetupOptions(configuration, optionsBuilder);
        return optionsBuilder;
    }
}