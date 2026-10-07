using System.Reflection;

namespace Architecture.Tests;

/// <summary>The modules under test. Adding a module means adding one line here.</summary>
internal static class Modules
{
    // Layers live as folders/namespaces inside a module's implementation assembly (decision #9).
    private static readonly string[] Layers = ["Domain", "Application", "Infrastructure", "Endpoints"];

    public static readonly IReadOnlyList<Module> All =
    [
        new("Users", typeof(Users.UsersModule).Assembly),
        new("WorkItems", typeof(WorkItems.WorkItemsModule).Assembly),
    ];

    public static Module Get(string name) => All.Single(m => m.Name == name);

    public static TheoryData<string> Names() => new(All.Select(m => m.Name));

    public static TheoryData<string, string> OrderedPairs()
    {
        var pairs = new TheoryData<string, string>();
        foreach (var a in All)
        foreach (var b in All.Where(b => b != a))
            pairs.Add(a.Name, b.Name);
        return pairs;
    }

    internal sealed record Module(string Name, Assembly Implementation)
    {
        // Loaded by name: a Contracts project may be empty, so there is no type to anchor typeof() on.
        public Assembly Contracts { get; } = Assembly.Load($"{Name}.Contracts");

        public string ImplementationAssemblyName => Implementation.GetName().Name!;

        public string[] InternalNamespaces => Layers.Select(layer => $"{Name}.{layer}").ToArray();
    }
}
