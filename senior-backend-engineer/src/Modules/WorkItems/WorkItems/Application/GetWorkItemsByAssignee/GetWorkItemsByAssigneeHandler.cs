using BuildingBlocks.Cqrs;
using BuildingBlocks.Results;
using WorkItems.Domain;

namespace WorkItems.Application.GetWorkItemsByAssignee;

internal sealed class GetWorkItemsByAssigneeHandler(IWorkItemReadStore workItems)
    : IQueryHandler<GetWorkItemsByAssigneeQuery, Result<IReadOnlyList<WorkItemDto>>>
{
    public async Task<Result<IReadOnlyList<WorkItemDto>>> ExecuteAsync(
        GetWorkItemsByAssigneeQuery query, CancellationToken ct)
    {
        var assigneeId = AssigneeId.Create(query.AssigneeId);
        if (assigneeId.IsFailure)
            return assigneeId.Error;

        // Deliberately no IUserDirectory call: an unknown user simply has no work items (decision #3),
        // which keeps the read side inside WorkItems' own data.
        var items = await workItems.ListByAssigneeAsync(assigneeId.Value, ct);

        return Result<IReadOnlyList<WorkItemDto>>.Success(items);
    }
}
