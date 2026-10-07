using Users.Domain;

namespace Users.Application.GetUserById;

/// <summary>
/// Read-side port: projects straight to <see cref="UserDto"/> without loading the aggregate.
/// Implemented in Infrastructure so Application stays free of EF Core (decision #20).
/// </summary>
internal interface IUserReadStore
{
    Task<UserDto?> GetByIdAsync(UserId id, CancellationToken ct);
}
