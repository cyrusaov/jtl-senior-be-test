using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using WorkItems.Application.GetWorkItemsByAssignee;
using WorkItems.Domain;
using WorkItems.Infrastructure;

namespace WorkItems;

/// <summary>The module's single public entry point: the host calls this to plug WorkItems in.</summary>
public static class WorkItemsModule
{
    /// <remarks>Requires <c>Users.Contracts.IUserDirectory</c> to be registered (by the Users module).</remarks>
    public static IServiceCollection AddWorkItemsModule(this IServiceCollection services)
    {
        // One in-memory store per host instance rather than per process, so test hosts never share data.
        // Swapping to a real database means replacing only this line.
        var store = new InMemoryDatabaseRoot();
        services.AddDbContext<WorkItemsDbContext>(o => o.UseInMemoryDatabase(WorkItemsDbContext.Schema, store));

        services.AddScoped<IWorkItemRepository, WorkItemRepository>(); // write side
        services.AddScoped<IWorkItemReadStore, WorkItemReadStore>();   // read side

        return services;
    }
}
