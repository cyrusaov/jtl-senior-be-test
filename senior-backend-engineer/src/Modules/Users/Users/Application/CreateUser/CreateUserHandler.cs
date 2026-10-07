using BuildingBlocks.Cqrs;
using BuildingBlocks.Results;
using Users.Domain;

namespace Users.Application.CreateUser;

internal sealed class CreateUserHandler(IUserRepository users) : ICommandHandler<CreateUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> ExecuteAsync(CreateUserCommand command, CancellationToken ct)
    {
        var username = Username.Create(command.Username);
        if (username.IsFailure)
            return username.Error;

        // Set-level rule across all users: enforced here, not in the aggregate (decision #12).
        // Not race-safe on the in-memory store; a unique index on the normalized username closes that gap in SQL.
        if (await users.ExistsByUsernameAsync(username.Value, ct))
            return UserErrors.UsernameTaken;

        var user = User.Create(username.Value);
        await users.AddAsync(user, ct);

        return user.Id.Value;
    }
}
