using Microsoft.EntityFrameworkCore;
using Users.Domain;

namespace Users.Infrastructure;

/// <summary>The Users module's own store. No other module can see or join against it.</summary>
internal sealed class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options)
{
    public const string Schema = "users";

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ignored by the in-memory provider; gives schema-per-module once a SQL provider is plugged in.
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
