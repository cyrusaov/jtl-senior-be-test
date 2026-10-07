using BuildingBlocks.Cqrs;
using BuildingBlocks.Results;
using Users.Domain;

namespace Users.Application.GetUserById;

internal sealed class GetUserByIdHandler(IUserReadStore users) : IQueryHandler<GetUserByIdQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> ExecuteAsync(GetUserByIdQuery query, CancellationToken ct)
    {
        var user = await users.GetByIdAsync(query.Id, ct);
        if (user is null)
            return UserErrors.NotFound;

        return user;
    }
}
