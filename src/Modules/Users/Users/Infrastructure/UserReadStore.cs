using Microsoft.EntityFrameworkCore;
using Users.Application.GetUserById;
using Users.Domain;

namespace Users.Infrastructure;

internal sealed class UserReadStore(UsersDbContext db) : IUserReadStore
{
    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var userId = new UserId(id);

        // No tracking, no aggregate: select only the columns the read model needs.
        var row = await db.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new { u.Id, Username = u.Username.Value })
            .SingleOrDefaultAsync(ct);

        return row is null ? null : new UserDto(row.Id.Value, row.Username);
    }
}
