using Users.Contracts;
using WorkItems.Domain;

namespace WorkItems.Tests.Application;

/// <summary>Stands in for the Users module: WorkItems is tested without any Users code, only its contract.</summary>
internal sealed class FakeUserDirectory(params Guid[] existingUserIds) : IUserDirectory
{
    public int Calls { get; private set; }

    public Task<bool> ExistsAsync(Guid userId, CancellationToken ct)
    {
        Calls++;
        return Task.FromResult(existingUserIds.Contains(userId));
    }
}

internal sealed class FakeWorkItemRepository : IWorkItemRepository
{
    public List<WorkItem> Added { get; } = [];

    public Task AddAsync(WorkItem workItem, CancellationToken ct)
    {
        Added.Add(workItem);
        return Task.CompletedTask;
    }
}
