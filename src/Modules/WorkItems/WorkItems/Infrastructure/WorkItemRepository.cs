using WorkItems.Domain;

namespace WorkItems.Infrastructure;

internal sealed class WorkItemRepository(WorkItemsDbContext db) : IWorkItemRepository
{
    // One command touches one aggregate, so saving here is the unit of work (plan §5).
    public async Task AddAsync(WorkItem workItem, CancellationToken ct)
    {
        db.WorkItems.Add(workItem);
        await db.SaveChangesAsync(ct);
    }
}
