using BuildingBlocks.Cqrs;
using BuildingBlocks.Results;

namespace Users.Application.CreateUser;

/// <summary>Creates a user. Returns only the new user's id.</summary>
internal sealed record CreateUserCommand(string? Username) : ICommand<Result<Guid>>;
