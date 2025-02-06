using Domain.Entities.Process;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<Process> Processes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresEnum<ProcessStatus>();
    }
}
