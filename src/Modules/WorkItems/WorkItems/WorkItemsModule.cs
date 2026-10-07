using Microsoft.Extensions.DependencyInjection;

namespace WorkItems;

/// <summary>The module's single public entry point: the host calls this to plug WorkItems in.</summary>
public static class WorkItemsModule
{
    public static IServiceCollection AddWorkItemsModule(this IServiceCollection services)
    {
        return services;
    }
}
