using WorkItems.Application.CreateWorkItem;
using WorkItems.Domain;

namespace WorkItems.Tests.Application;

public sealed class CreateWorkItemHandlerTests
{
    private static readonly Guid ExistingUser = Guid.NewGuid();

    private readonly FakeWorkItemRepository _repository = new();
    private readonly FakeUserDirectory _users = new(ExistingUser);

    private Task<BuildingBlocks.Results.Result<Guid>> Handle(string? name, Guid? assigneeId) =>
        new CreateWorkItemHandler(_repository, _users)
            .ExecuteAsync(new CreateWorkItemCommand(name, assigneeId), CancellationToken.None);

    [Fact]
    public async Task Existing_assignee_creates_and_stores_the_work_item()
    {
        var result = await Handle("Write the README", ExistingUser);

        result.IsSuccess.Should().BeTrue();
        var stored = _repository.Added.Should().ContainSingle().Subject;
        stored.Id.Value.Should().Be(result.Value);
        stored.Name.Value.Should().Be("Write the README");
        stored.AssigneeId.Value.Should().Be(ExistingUser);
    }

    [Fact]
    public async Task Unknown_assignee_is_rejected_and_nothing_is_stored()
    {
        var result = await Handle("Write the README", Guid.NewGuid());

        result.Error.Should().Be(WorkItemErrors.AssigneeNotFound);
        _repository.Added.Should().BeEmpty();
    }

    [Fact]
    public async Task Invalid_input_is_rejected_before_asking_the_users_module()
    {
        var result = await Handle("   ", ExistingUser);

        result.Error.Should().Be(WorkItemErrors.NameRequired);
        _users.Calls.Should().Be(0);
        _repository.Added.Should().BeEmpty();
    }

    [Fact]
    public async Task Missing_assignee_is_a_validation_error_not_a_lookup()
    {
        var result = await Handle("Write the README", null);

        result.Error.Should().Be(WorkItemErrors.AssigneeRequired);
        _users.Calls.Should().Be(0);
    }
}
