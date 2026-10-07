using Microsoft.EntityFrameworkCore;
using Users.Domain;

namespace Users.Infrastructure;

internal sealed class UserRepository(UsersDbContext db) : IUserRepository
{
    public Task<bool> ExistsByUsernameAsync(Username username, CancellationToken ct) =>
        db.Users.AnyAsync(u => u.Username.NormalizedValue == username.NormalizedValue, ct);

    // One command touches one aggregate, so saving here is the unit of work (plan §5).
    public async Task AddAsync(User user, CancellationToken ct)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
    }
}
