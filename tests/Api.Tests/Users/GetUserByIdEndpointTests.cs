using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.Tests.Users;

public sealed class GetUserByIdEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Existing_user_returns_200_with_username_as_entered()
    {
        var username = UsersApi.UniqueUsername().ToUpperInvariant();
        var id = await _client.CreateUserAsync(username);

        var response = await _client.GetAsync($"/users/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        user.Should().Be(new UserResponse(id, username));
    }

    [Fact]
    public async Task Location_from_create_resolves_to_the_created_user()
    {
        var create = await _client.PostAsJsonAsync("/users", new { username = UsersApi.UniqueUsername() });

        var response = await _client.GetAsync(create.Headers.Location);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Unknown_id_returns_404_problem()
    {
        var response = await _client.GetAsync($"/users/{Guid.NewGuid()}");

        await response.ShouldBeProblemAsync(HttpStatusCode.NotFound, "generalErrors", "users.not_found");
    }

    [Fact]
    public async Task Malformed_id_returns_400_problem()
    {
        var response = await _client.GetAsync("/users/not-a-guid");

        await response.ShouldBeProblemAsync(HttpStatusCode.BadRequest, "id");
    }

    private sealed record UserResponse(Guid Id, string Username);
}
