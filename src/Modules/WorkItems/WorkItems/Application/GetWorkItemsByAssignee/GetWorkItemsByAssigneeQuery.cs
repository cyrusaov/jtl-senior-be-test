using BuildingBlocks.Cqrs;
using BuildingBlocks.Results;

namespace WorkItems.Application.GetWorkItemsByAssignee;

/// <summary>Lists the work items assigned to a user, ordered by name then id. Never changes state.</summary>
internal sealed record GetWorkItemsByAssigneeQuery(Guid? AssigneeId) : IQuery<Result<IReadOnlyList<WorkItemDto>>>;

/// <summary>Read model returned to callers; never the aggregate itself.</summary>
internal sealed record WorkItemDto(Guid Id, string Name, Guid AssigneeId);
