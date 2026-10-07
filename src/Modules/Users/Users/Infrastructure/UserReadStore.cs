using Microsoft.EntityFrameworkCore;
using Users.Application.GetUserById;
using Users.Domain;

namespace Users.Infrastructure;

internal sealed class UserReadStore(UsersDbContext db) : IUserReadStore
{
    public async Task<UserDto?> GetByIdAsync(UserId id, CancellationToken ct)
    {
        // No tracking, no aggregate: select only the columns the read model needs.
        var row = await db.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new { u.Id, Username = u.Username.Value })
            .SingleOrDefaultAsync(ct);

        return row is null ? null : new UserDto(row.Id.Value, row.Username);
    }
}
