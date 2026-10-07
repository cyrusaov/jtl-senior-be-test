using BuildingBlocks.Http;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Users.Application.GetUserById;

namespace Users.Endpoints.GetUserById;

internal sealed record GetUserByIdRequest(Guid Id);

internal sealed record GetUserByIdResponse(Guid Id, string Username);

internal sealed class GetUserByIdEndpoint : Endpoint<GetUserByIdRequest, GetUserByIdResponse>
{
    public override void Configure()
    {
        // No {id:guid} route constraint on purpose: a malformed id should be a 400, not a 404 "no such route".
        Get("/users/{id}");
        AllowAnonymous();
        Description(b => b
            .ProducesProblemDetails(StatusCodes.Status400BadRequest)
            .ProducesProblemDetails(StatusCodes.Status404NotFound));
        Summary(s =>
        {
            s.Summary = "Get a user by id";
            s.Responses[StatusCodes.Status200OK] = "The user.";
            s.Responses[StatusCodes.Status400BadRequest] = "The id is not a valid GUID.";
            s.Responses[StatusCodes.Status404NotFound] = "No user with this id.";
        });
    }

    public override async Task HandleAsync(GetUserByIdRequest req, CancellationToken ct)
    {
        var result = await new GetUserByIdQuery(req.Id).ExecuteAsync(ct);

        if (result.IsFailure)
        {
            await Send.SendErrorAsync(result.Error, ct);
            return;
        }

        await Send.OkAsync(new GetUserByIdResponse(result.Value.Id, result.Value.Username), ct);
    }
}
