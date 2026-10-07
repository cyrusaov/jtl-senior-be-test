using FastEndpoints;
using FastEndpoints.Swagger;
using Users;
using WorkItems;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddFastEndpoints(o =>
    {
        // Explicit over magic: the host declares which modules exist, FastEndpoints scans only those.
        o.DisableAutoDiscovery = true;
        o.Assemblies = [typeof(UsersModule).Assembly, typeof(WorkItemsModule).Assembly];
    })
    .SwaggerDocument(o =>
    {
        o.ShortSchemaNames = true; // "CreateUserResponse", not "UsersEndpointsCreateUserCreateUserResponse"
        o.DocumentSettings = s =>
        {
            s.Title = "JTL Users & Work Items API";
            s.Version = "v1";
        };
    })
    .AddUsersModule()
    .AddWorkItemsModule();

var app = builder.Build();

app.UseFastEndpoints(c => c.Errors.UseProblemDetails(p => p.IndicateErrorCode = true))
   .UseSwaggerGen();

app.Run();

// Exposes the entry point to WebApplicationFactory<Program> in the API tests.
public partial class Program;
