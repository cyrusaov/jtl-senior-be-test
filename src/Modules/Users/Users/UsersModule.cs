using Microsoft.Extensions.DependencyInjection;

namespace Users;

/// <summary>The module's single public entry point: the host calls this to plug Users in.</summary>
public static class UsersModule
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services)
    {
        return services;
    }
}
