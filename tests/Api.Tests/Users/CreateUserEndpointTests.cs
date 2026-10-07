using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.Tests.Users;

public sealed class CreateUserEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Valid_username_returns_201_with_id_and_location()
    {
        var response = await _client.PostAsJsonAsync("/users", new { username = UniqueUsername() });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<CreatedResponse>();
        body!.Id.Should().NotBeEmpty();
        response.Headers.Location!.ToString().Should().Be($"/users/{body.Id}");
    }

    [Fact]
    public async Task Duplicate_username_ignoring_case_returns_409_problem()
    {
        var username = UniqueUsername();
        (await _client.PostAsJsonAsync("/users", new { username })).EnsureSuccessStatusCode();

        var response = await _client.PostAsJsonAsync("/users", new { username = username.ToUpperInvariant() });

        await ShouldBeProblem(response, HttpStatusCode.Conflict, "users.username.taken");
    }

    [Theory]
    [InlineData("ab", "users.username.invalid_length")]
    [InlineData("not valid!", "users.username.invalid_characters")]
    [InlineData(null, "users.username.required")]
    public async Task Invalid_username_returns_400_problem(string? username, string expectedCode)
    {
        var response = await _client.PostAsJsonAsync("/users", new { username });

        await ShouldBeProblem(response, HttpStatusCode.BadRequest, expectedCode);
    }

    private static async Task ShouldBeProblem(HttpResponseMessage response, HttpStatusCode status, string code)
    {
        response.StatusCode.Should().Be(status);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        var problem = await response.Content.ReadFromJsonAsync<Problem>();
        problem!.Status.Should().Be((int)status);
        problem.Errors.Should().ContainSingle(e => e.Name == "username" && e.Code == code);
    }

    // The fixture's host is shared by the tests in this class, so each test uses its own username.
    private static string UniqueUsername() => $"user_{Guid.NewGuid():N}"[..20];

    private sealed record CreatedResponse(Guid Id);

    private sealed record Problem(int Status, IReadOnlyList<ProblemError> Errors);

    private sealed record ProblemError(string Name, string Reason, string? Code);
}
