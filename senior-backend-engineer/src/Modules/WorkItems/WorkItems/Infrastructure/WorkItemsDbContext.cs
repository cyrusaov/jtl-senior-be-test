using Microsoft.EntityFrameworkCore;
using WorkItems.Domain;

namespace WorkItems.Infrastructure;

/// <summary>The WorkItems module's own store. Holds assignee ids only; never reads Users data.</summary>
internal sealed class WorkItemsDbContext(DbContextOptions<WorkItemsDbContext> options) : DbContext(options)
{
    public const string Schema = "work_items";

    public DbSet<WorkItem> WorkItems => Set<WorkItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ignored by the in-memory provider; gives schema-per-module once a SQL provider is plugged in.
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfiguration(new WorkItemConfiguration());
    }
}
