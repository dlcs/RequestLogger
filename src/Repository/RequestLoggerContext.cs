using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Repository.Models;


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
    public RequestLoggerContext(DbContextOptions<RequestLoggerContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HttpRequest>(builder =>
        {
            builder.HasKey(i => i.Id);
            builder.HasAnnotation("Npgsql:ValueGenerationStrategy",
                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
            builder.Property(p => p.Body).HasColumnType("jsonb");
            builder.Property(p => p.Headers).HasColumnType("jsonb");
        });
    }

    public virtual DbSet<HttpRequest> Requests { get; set; } = null!;
}