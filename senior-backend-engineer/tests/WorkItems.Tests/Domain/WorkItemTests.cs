using WorkItems.Domain;

namespace WorkItems.Tests.Domain;

public sealed class WorkItemTests
{
    private static readonly WorkItemName Name = WorkItemName.Create("Write the README").Value;
    private static readonly AssigneeId Assignee = AssigneeId.Create(Guid.NewGuid()).Value;

    [Fact]
    public void Create_assigns_a_new_identity_and_keeps_name_and_assignee()
    {
        var workItem = WorkItem.Create(Name, Assignee);

        workItem.Id.Value.Should().NotBeEmpty();
        workItem.Name.Should().Be(Name);
        workItem.AssigneeId.Should().Be(Assignee);
    }

    [Fact]
    public void Each_created_work_item_gets_a_distinct_identity() =>
        WorkItem.Create(Name, Assignee).Id.Should().NotBe(WorkItem.Create(Name, Assignee).Id);

    [Fact]
    public void Exposes_no_public_setters() =>
        typeof(WorkItem).GetProperties().Where(p => p.SetMethod?.IsPublic == true).Should().BeEmpty();
}
