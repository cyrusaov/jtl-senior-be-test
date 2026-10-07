using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Users.Domain;
using Users.Infrastructure;

namespace Users;

/// <summary>The module's single public entry point: the host calls this to plug Users in.</summary>
public static class UsersModule
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services)
    {
        // One in-memory store per host instance rather than per process, so test hosts never share data.
        // Swapping to a real database means replacing only this line.
        var store = new InMemoryDatabaseRoot();
        services.AddDbContext<UsersDbContext>(o => o.UseInMemoryDatabase(UsersDbContext.Schema, store));

        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
