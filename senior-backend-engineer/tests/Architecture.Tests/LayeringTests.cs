using NetArchTest.Rules;

namespace Architecture.Tests;

/// <summary>
/// Dependency direction inside a module: Endpoints → Application → Domain, with Infrastructure implementing
/// the ports that Domain and Application define. Layers are namespaces in one assembly (decision #9),
/// so these tests are what keeps them honest.
/// </summary>
public sealed class LayeringTests
{
    private const string EfCore = "Microsoft.EntityFrameworkCore";
    private const string AspNetCore = "Microsoft.AspNetCore";

    // Layer → what it must not depend on. "{m}" is replaced with the module name.
    private static readonly Dictionary<string, string[]> Forbidden = new()
    {
        ["Domain"] = ["{m}.Application", "{m}.Infrastructure", "{m}.Endpoints"],
        // FastEndpoints is allowed here: the CQRS markers derive from its command bus (decision #10).
        ["Application"] = ["{m}.Infrastructure", "{m}.Endpoints", EfCore, AspNetCore, "BuildingBlocks.Http"],
        ["Infrastructure"] = ["{m}.Endpoints"],
        // Endpoints only map HTTP to commands/queries: no domain objects, no persistence.
        ["Endpoints"] = ["{m}.Domain", "{m}.Infrastructure", EfCore],
    };

    public static TheoryData<string, string> ModulesAndLayers()
    {
        var data = new TheoryData<string, string>();
        foreach (var module in Modules.All)
        foreach (var layer in Forbidden.Keys)
            data.Add(module.Name, layer);
        return data;
    }

    [Theory]
    [MemberData(nameof(ModulesAndLayers))]
    public void Layer_respects_dependency_direction(string moduleName, string layer)
    {
        var assembly = Modules.Get(moduleName).Implementation;
        var layerNamespace = $"{moduleName}.{layer}";
        var forbidden = Forbidden[layer].Select(f => f.Replace("{m}", moduleName)).ToArray();

        // Guard against a vacuous pass, e.g. after a namespace rename.
        Assert.NotEmpty(Types.InAssembly(assembly).That().ResideInNamespace(layerNamespace).GetTypes());

        var result = Types.InAssembly(assembly)
            .That().ResideInNamespace(layerNamespace)
            .ShouldNot().HaveDependencyOnAny(forbidden)
            .GetResult();

        Assert.True(result.IsSuccessful,
            result.Describe($"{layerNamespace} must not depend on {string.Join(", ", forbidden)}"));
    }
}
