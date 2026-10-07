using BuildingBlocks.Http;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Users.Application.CreateUser;
using Users.Endpoints.GetUserById;

namespace Users.Endpoints.CreateUser;

internal sealed record CreateUserRequest(string? Username);

internal sealed record CreateUserResponse(Guid Id);

internal sealed class CreateUserEndpoint : Endpoint<CreateUserRequest, CreateUserResponse>
{
    public override void Configure()
    {
        Post("/users");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Create a user";
            s.Responses[StatusCodes.Status201Created] = "User created; Location points to the new user.";
            s.Responses[StatusCodes.Status400BadRequest] = "Username is missing or invalid.";
            s.Responses[StatusCodes.Status409Conflict] = "Username is already taken (case-insensitive).";
        });
    }

    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var result = await new CreateUserCommand(req.Username).ExecuteAsync(ct);

        if (result.IsFailure)
        {
            await Send.SendErrorAsync(result.Error, ct);
            return;
        }

        // Location is derived from the GET endpoint's route, so the two can't drift apart (decision #18).
        await Send.CreatedAtAsync<GetUserByIdEndpoint>(
            new { id = result.Value }, new CreateUserResponse(result.Value), cancellation: ct);
    }
}
