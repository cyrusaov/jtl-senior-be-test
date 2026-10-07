using Microsoft.EntityFrameworkCore;
using WorkItems.Application.GetWorkItemsByAssignee;
using WorkItems.Domain;

namespace WorkItems.Infrastructure;

internal sealed class WorkItemReadStore(WorkItemsDbContext db) : IWorkItemReadStore
{
    public async Task<IReadOnlyList<WorkItemDto>> ListByAssigneeAsync(AssigneeId assigneeId, CancellationToken ct)
    {
        // No tracking, no aggregate: select only the columns the read model needs.
        var rows = await db.WorkItems
            .AsNoTracking()
            .Where(w => w.AssigneeId == assigneeId)
            .Select(w => new { w.Id, w.Name })
            .ToListAsync(ct);

        // Ordered after projection because Name is a converted value object. With a SQL provider this would
        // order on the column in the query instead.
        return rows
            .Select(r => new WorkItemDto(r.Id.Value, r.Name.Value, assigneeId.Value))
            .OrderBy(dto => dto.Name, StringComparer.Ordinal)
            .ThenBy(dto => dto.Id)
            .ToList();
    }
}
