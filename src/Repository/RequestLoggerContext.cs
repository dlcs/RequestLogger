using Microsoft.EntityFrameworkCore;
using Repository.Models;
using Serilog;

namespace Repository;

public class RequestLoggerContext : DbContext
{
    /// <summary>
    /// Context class for entity framework
    /// </summary>
    public RequestLoggerContext()
    {
    }

    /// <summary>
    /// Context class for entity framework
    /// </summary>
    /// <param name="options">The db context options</param>
    /// <param name="connectionString">The postgres database connection string</param>
    public RequestLoggerContext(DbContextOptions<RequestLoggerContext> options) 
        : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql();
        }
        
        optionsBuilder.UseSnakeCaseNamingConvention();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseSerialColumns();
    }

    public virtual DbSet<HttpRequest> Requests { get; set; } = null!;
}