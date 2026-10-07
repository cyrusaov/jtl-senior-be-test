using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Users.Contracts;

namespace Api.Tests.Users;

/// <summary>Exercises IUserDirectory exactly as another module sees it: resolved from DI, by its public contract.</summary>
public sealed class UserDirectoryContractTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Existing_user_exists()
    {
        var id = await factory.CreateClient().CreateUserAsync();

        (await ExistsAsync(id)).Should().BeTrue();
    }

    [Fact]
    public async Task Unknown_user_does_not_exist()
    {
        (await ExistsAsync(Guid.NewGuid())).Should().BeFalse();
    }

    private async Task<bool> ExistsAsync(Guid userId)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<IUserDirectory>().ExistsAsync(userId, CancellationToken.None);
    }
}
