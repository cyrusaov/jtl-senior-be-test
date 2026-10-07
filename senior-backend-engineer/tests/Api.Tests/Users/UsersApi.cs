using System.Net.Http.Json;

namespace Api.Tests.Users;

/// <summary>Small HTTP helpers for arranging Users state in API tests (reused by WorkItems tests later).</summary>
internal static class UsersApi
{
    // Hosts are shared by the tests in a class, so every test uses its own username.
    public static string UniqueUsername() => $"user_{Guid.NewGuid():N}"[..20];

    public static async Task<Guid> CreateUserAsync(this HttpClient client, string? username = null)
    {
        var response = await client.PostAsJsonAsync("/users", new { username = username ?? UniqueUsername() });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CreatedResponse>())!.Id;
    }

    internal sealed record CreatedResponse(Guid Id);
}
