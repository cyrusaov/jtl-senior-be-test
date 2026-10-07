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
        var response = await _client.PostAsJsonAsync("/users", new { username = UsersApi.UniqueUsername() });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<UsersApi.CreatedResponse>();
        body!.Id.Should().NotBeEmpty();
        response.Headers.Location!.ToString().Should().Be($"/users/{body.Id}");
    }

    [Fact]
    public async Task Duplicate_username_ignoring_case_returns_409_problem()
    {
        var username = UsersApi.UniqueUsername();
        await _client.CreateUserAsync(username);

        var response = await _client.PostAsJsonAsync("/users", new { username = username.ToUpperInvariant() });

        await response.ShouldBeProblemAsync(HttpStatusCode.Conflict, "username", "users.username.taken");
    }

    [Theory]
    [InlineData("ab", "users.username.invalid_length")]
    [InlineData("not valid!", "users.username.invalid_characters")]
    [InlineData(null, "users.username.required")]
    public async Task Invalid_username_returns_400_problem(string? username, string expectedCode)
    {
        var response = await _client.PostAsJsonAsync("/users", new { username });

        await response.ShouldBeProblemAsync(HttpStatusCode.BadRequest, "username", expectedCode);
    }
}
