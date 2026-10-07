using BuildingBlocks.Cqrs;
using BuildingBlocks.Results;

namespace Users.Application.GetUserById;

/// <summary>Reads one user. Never changes state.</summary>
internal sealed record GetUserByIdQuery(Guid Id) : IQuery<Result<UserDto>>;

/// <summary>Read model returned to callers; never the aggregate itself.</summary>
internal sealed record UserDto(Guid Id, string Username);
