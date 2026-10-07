using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkItems.Domain;

namespace WorkItems.Infrastructure;

internal sealed class WorkItemConfiguration : IEntityTypeConfiguration<WorkItem>
{
    public void Configure(EntityTypeBuilder<WorkItem> workItem)
    {
        workItem.ToTable("work_items");

        workItem.HasKey(w => w.Id);
        workItem.Property(w => w.Id)
            .HasConversion(id => id.Value, value => new WorkItemId(value))
            .ValueGeneratedNever();

        // Persisted values were validated on the way in, so rehydrating through the factory cannot fail.
        workItem.Property(w => w.Name)
            .HasConversion(name => name.Value, value => WorkItemName.Create(value).Value)
            .HasColumnName("name")
            .HasMaxLength(WorkItemName.MaxLength)
            .IsRequired();

        // A plain id, not a foreign key: users live in another module's store (no cross-module joins).
        workItem.Property(w => w.AssigneeId)
            .HasConversion(assignee => assignee.Value, value => AssigneeId.Create(value).Value)
            .HasColumnName("assignee_id")
            .IsRequired();

        // Supports the "list by assignee" query (GetWorkItemsByAssignee).
        workItem.HasIndex(w => w.AssigneeId);
    }
}
