using BuildingBlocks.Results;

namespace WorkItems.Domain;

/// <summary>
/// Who a work item is assigned to, from the WorkItems module's point of view. Deliberately not Users' <c>UserId</c>:
/// WorkItems only knows "a user id that the Users module confirmed exists" (via <c>IUserDirectory</c>).
/// A class rather than a struct so an empty assignee can only be built through <see cref="Create"/>.
/// </summary>
internal sealed record AssigneeId
{
    private AssigneeId(Guid value) => Value = value;

    public Guid Value { get; }

    public static Result<AssigneeId> Create(Guid? value)
    {
        if (value is null || value == Guid.Empty)
            return WorkItemErrors.AssigneeRequired;

        return new AssigneeId(value.Value);
    }

    public override string ToString() => Value.ToString();
}
