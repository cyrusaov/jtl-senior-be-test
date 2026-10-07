using WorkItems.Domain;

namespace WorkItems.Application.GetWorkItemsByAssignee;

/// <summary>
/// Read-side port: projects straight to <see cref="WorkItemDto"/> without loading aggregates.
/// Implemented in Infrastructure so Application stays free of EF Core (decision #20).
/// </summary>
internal interface IWorkItemReadStore
{
    /// <summary>Ordered by name, then id (decision #24).</summary>
    Task<IReadOnlyList<WorkItemDto>> ListByAssigneeAsync(AssigneeId assigneeId, CancellationToken ct);
}
