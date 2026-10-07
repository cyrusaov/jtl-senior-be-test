namespace WorkItems.Domain;

/// <summary>Aggregate root of the WorkItems module. Immutable after creation: no update use case exists yet.</summary>
internal sealed class WorkItem
{
    // Required by EF Core to materialize the aggregate; not usable by application code.
    private WorkItem()
    {
        Name = null!;
        AssigneeId = null!;
    }

    private WorkItem(WorkItemId id, WorkItemName name, AssigneeId assigneeId)
    {
        Id = id;
        Name = name;
        AssigneeId = assigneeId;
    }

    public WorkItemId Id { get; private set; }

    public WorkItemName Name { get; private set; }

    public AssigneeId AssigneeId { get; private set; }

    /// <summary>
    /// Creates a work item. Both inputs are already-valid value objects, so the aggregate cannot be built in an
    /// invalid state. Whether the assignee exists is a cross-module rule, checked by CreateWorkItemHandler (decision #2).
    /// </summary>
    public static WorkItem Create(WorkItemName name, AssigneeId assigneeId) =>
        new(WorkItemId.New(), name, assigneeId);
}
