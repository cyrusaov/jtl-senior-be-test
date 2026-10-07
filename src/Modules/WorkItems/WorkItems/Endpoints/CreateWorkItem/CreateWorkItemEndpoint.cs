using BuildingBlocks.Http;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using WorkItems.Application.CreateWorkItem;

namespace WorkItems.Endpoints.CreateWorkItem;

// AssigneeId is nullable so a missing value reaches the domain as "required" instead of binding to Guid.Empty.
internal sealed record CreateWorkItemRequest(string? Name, Guid? AssigneeId);

internal sealed record CreateWorkItemResponse(Guid Id);

internal sealed class CreateWorkItemEndpoint : Endpoint<CreateWorkItemRequest, CreateWorkItemResponse>
{
    public override void Configure()
    {
        Post("/work-items");
        AllowAnonymous();
        Description(b => b
            .ClearDefaultProduces(StatusCodes.Status200OK)
            .Produces<CreateWorkItemResponse>(StatusCodes.Status201Created)
            .ProducesProblemDetails(StatusCodes.Status400BadRequest)
            .ProducesProblemDetails(StatusCodes.Status422UnprocessableEntity));
        Summary(s =>
        {
            s.Summary = "Create a work item assigned to a user";
            s.Responses[StatusCodes.Status201Created] = "Work item created.";
            s.Responses[StatusCodes.Status400BadRequest] = "Name or assignee id is missing or invalid.";
            s.Responses[StatusCodes.Status422UnprocessableEntity] = "The assignee does not exist.";
        });
    }

    public override async Task HandleAsync(CreateWorkItemRequest req, CancellationToken ct)
    {
        var result = await new CreateWorkItemCommand(req.Name, req.AssigneeId).ExecuteAsync(ct);

        if (result.IsFailure)
        {
            await Send.SendErrorAsync(result.Error, ct);
            return;
        }

        // No Location header: the API has no GET /work-items/{id} to point at (plan §6).
        await Send.ResponseAsync(new CreateWorkItemResponse(result.Value), StatusCodes.Status201Created, ct);
    }
}
