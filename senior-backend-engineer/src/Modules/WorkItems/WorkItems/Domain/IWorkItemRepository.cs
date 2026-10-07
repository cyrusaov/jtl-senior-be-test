namespace WorkItems.Domain;

/// <summary>Write-side access to <see cref="WorkItem"/> aggregates. Reads go through query handlers (decision #20).</summary>
internal interface IWorkItemRepository
{
    Task AddAsync(WorkItem workItem, CancellationToken ct);
}
