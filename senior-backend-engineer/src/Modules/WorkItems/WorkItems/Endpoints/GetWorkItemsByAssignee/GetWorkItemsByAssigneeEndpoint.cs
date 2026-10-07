using BuildingBlocks.Http;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using WorkItems.Application.GetWorkItemsByAssignee;

namespace WorkItems.Endpoints.GetWorkItemsByAssignee;

// AssigneeId is nullable so a missing query parameter reaches the domain as "required" (400), not Guid.Empty.
internal sealed record GetWorkItemsByAssigneeRequest(Guid? AssigneeId);

internal sealed record WorkItemResponse(Guid Id, string Name, Guid AssigneeId);

internal sealed class GetWorkItemsByAssigneeEndpoint
    : Endpoint<GetWorkItemsByAssigneeRequest, IReadOnlyList<WorkItemResponse>>
{
    public override void Configure()
    {
        Get("/work-items");
        AllowAnonymous();
        Description(b => b.ProducesProblemDetails(StatusCodes.Status400BadRequest));
        Summary(s =>
        {
            s.Summary = "List the work items assigned to a user";
            s.Params["assigneeId"] = "Id of the assigned user (required).";
            s.Responses[StatusCodes.Status200OK] = "Work items ordered by name; empty if the user has none or is unknown.";
            s.Responses[StatusCodes.Status400BadRequest] = "assigneeId is missing or not a valid GUID.";
        });
    }

    public override async Task HandleAsync(GetWorkItemsByAssigneeRequest req, CancellationToken ct)
    {
        var result = await new GetWorkItemsByAssigneeQuery(req.AssigneeId).ExecuteAsync(ct);

        if (result.IsFailure)
        {
            await Send.SendErrorAsync(result.Error, ct);
            return;
        }

        await Send.OkAsync(result.Value.Select(w => new WorkItemResponse(w.Id, w.Name, w.AssigneeId)).ToList(), ct);
    }
}
