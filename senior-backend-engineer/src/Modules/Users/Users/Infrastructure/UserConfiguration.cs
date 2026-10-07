using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain;

namespace Users.Infrastructure;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> user)
    {
        user.ToTable("users");

        user.HasKey(u => u.Id);
        user.Property(u => u.Id)
            .HasConversion(id => id.Value, value => new UserId(value))
            .ValueGeneratedNever();

        user.OwnsOne(u => u.Username, username =>
        {
            username.Property(n => n.Value)
                .HasColumnName("username")
                .HasMaxLength(Username.MaxLength)
                .IsRequired();

            username.Property(n => n.NormalizedValue)
                .HasColumnName("normalized_username")
                .HasMaxLength(Username.MaxLength)
                .IsRequired();

            // Not enforced by the in-memory provider; the real race-safe uniqueness guard on SQL.
            username.HasIndex(n => n.NormalizedValue).IsUnique();
        });
        user.Navigation(u => u.Username).IsRequired();
    }
}
