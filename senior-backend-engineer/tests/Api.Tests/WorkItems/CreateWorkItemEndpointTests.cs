using System.Net;
using System.Net.Http.Json;
using Api.Tests.Users;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.Tests.WorkItems;

/// <summary>Crosses both modules through HTTP: the user is created by Users, then referenced by WorkItems.</summary>
public sealed class CreateWorkItemEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Existing_assignee_returns_201_with_id()
    {
        var assigneeId = await _client.CreateUserAsync();

        var response = await _client.PostAsJsonAsync("/work-items", new { name = "Write the README", assigneeId });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        (await response.Content.ReadFromJsonAsync<CreatedResponse>())!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Unknown_assignee_returns_422_problem()
    {
        var response = await _client.PostAsJsonAsync(
            "/work-items", new { name = "Write the README", assigneeId = Guid.NewGuid() });

        await response.ShouldBeProblemAsync(
            HttpStatusCode.UnprocessableEntity, "assigneeId", "workitems.assignee.not_found");
    }

    [Fact]
    public async Task Missing_assignee_returns_400_problem()
    {
        var response = await _client.PostAsJsonAsync("/work-items", new { name = "Write the README" });

        await response.ShouldBeProblemAsync(HttpStatusCode.BadRequest, "assigneeId", "workitems.assignee.required");
    }

    [Fact]
    public async Task Blank_name_returns_400_problem()
    {
        var assigneeId = await _client.CreateUserAsync();

        var response = await _client.PostAsJsonAsync("/work-items", new { name = " ", assigneeId });

        await response.ShouldBeProblemAsync(HttpStatusCode.BadRequest, "name", "workitems.name.required");
    }

    [Fact]
    public async Task Malformed_assignee_id_returns_400_problem()
    {
        var response = await _client.PostAsJsonAsync(
            "/work-items", new { name = "Write the README", assigneeId = "not-a-guid" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
    }

    private sealed record CreatedResponse(Guid Id);
}
