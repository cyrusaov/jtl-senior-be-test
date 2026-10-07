using Microsoft.EntityFrameworkCore;
using Users.Contracts;
using Users.Domain;

namespace Users.Infrastructure;

/// <summary>Adapter from the public <see cref="IUserDirectory"/> contract to the module's own store.</summary>
internal sealed class UserDirectory(UsersDbContext db) : IUserDirectory
{
    public Task<bool> ExistsAsync(Guid userId, CancellationToken ct)
    {
        var id = new UserId(userId);
        return db.Users.AsNoTracking().AnyAsync(u => u.Id == id, ct);
    }
}
