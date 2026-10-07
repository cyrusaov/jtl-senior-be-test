using System.Net;
using System.Net.Http.Json;
using Api.Tests.Users;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.Tests.WorkItems;

public sealed class GetWorkItemsByAssigneeEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Lists_only_the_assignees_work_items_ordered_by_name()
    {
        var assignee = await _client.CreateUserAsync();
        var someoneElse = await _client.CreateUserAsync();
        var zebra = await CreateWorkItemAsync("Zebra task", assignee);
        var apple = await CreateWorkItemAsync("Apple task", assignee);
        await CreateWorkItemAsync("Not mine", someoneElse);

        var items = await _client.GetFromJsonAsync<List<WorkItemResponse>>($"/work-items?assigneeId={assignee}");

        items.Should().Equal(
            new WorkItemResponse(apple, "Apple task", assignee),
            new WorkItemResponse(zebra, "Zebra task", assignee));
    }

    [Fact]
    public async Task Unknown_user_returns_200_with_empty_list()
    {
        var response = await _client.GetAsync($"/work-items?assigneeId={Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<List<WorkItemResponse>>()).Should().BeEmpty();
    }

    [Fact]
    public async Task Missing_assignee_id_returns_400_problem()
    {
        var response = await _client.GetAsync("/work-items");

        await response.ShouldBeProblemAsync(HttpStatusCode.BadRequest, "assigneeId", "workitems.assignee.required");
    }

    [Fact]
    public async Task Malformed_assignee_id_returns_400_problem()
    {
        var response = await _client.GetAsync("/work-items?assigneeId=not-a-guid");

        await response.ShouldBeProblemAsync(HttpStatusCode.BadRequest, "assigneeId");
    }

    private async Task<Guid> CreateWorkItemAsync(string name, Guid assigneeId)
    {
        var response = await _client.PostAsJsonAsync("/work-items", new { name, assigneeId });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<WorkItemResponse>())!.Id;
    }

    private sealed record WorkItemResponse(Guid Id, string Name, Guid AssigneeId);
}
