using BuildingBlocks.Cqrs;
using BuildingBlocks.Results;

namespace WorkItems.Application.CreateWorkItem;

/// <summary>Creates a work item assigned to an existing user. Returns only the new work item's id.</summary>
internal sealed record CreateWorkItemCommand(string? Name, Guid? AssigneeId) : ICommand<Result<Guid>>;
