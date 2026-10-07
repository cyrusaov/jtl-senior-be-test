using WorkItems.Domain;

namespace WorkItems.Tests.Domain;

public sealed class AssigneeIdTests
{
    [Fact]
    public void Accepts_a_non_empty_id()
    {
        var id = Guid.NewGuid();

        AssigneeId.Create(id).Value.Value.Should().Be(id);
    }

    [Fact]
    public void Rejects_a_missing_id() =>
        AssigneeId.Create(null).Error.Should().Be(WorkItemErrors.AssigneeRequired);

    [Fact]
    public void Rejects_the_empty_id() =>
        AssigneeId.Create(Guid.Empty).Error.Should().Be(WorkItemErrors.AssigneeRequired);

    [Fact]
    public void Has_value_equality()
    {
        var id = Guid.NewGuid();

        AssigneeId.Create(id).Value.Should().Be(AssigneeId.Create(id).Value);
    }
}
