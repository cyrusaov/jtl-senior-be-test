using BuildingBlocks.Cqrs;
using BuildingBlocks.Results;
using Users.Contracts;
using WorkItems.Domain;

namespace WorkItems.Application.CreateWorkItem;

internal sealed class CreateWorkItemHandler(IWorkItemRepository workItems, IUserDirectory users)
    : ICommandHandler<CreateWorkItemCommand, Result<Guid>>
{
    public async Task<Result<Guid>> ExecuteAsync(CreateWorkItemCommand command, CancellationToken ct)
    {
        var name = WorkItemName.Create(command.Name);
        if (name.IsFailure)
            return name.Error;

        var assigneeId = AssigneeId.Create(command.AssigneeId);
        if (assigneeId.IsFailure)
            return assigneeId.Error;

        // Cross-module rule: asked through Users' public contract only, never its data (decisions #2, #23).
        if (!await users.ExistsAsync(assigneeId.Value.Value, ct))
            return WorkItemErrors.AssigneeNotFound;

        var workItem = WorkItem.Create(name.Value, assigneeId.Value);
        await workItems.AddAsync(workItem, ct);

        return workItem.Id.Value;
    }
}
