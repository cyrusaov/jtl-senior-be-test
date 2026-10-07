using NetArchTest.Rules;

namespace Architecture.Tests;

/// <summary>A module may talk to another module only through that module's *.Contracts assembly.</summary>
public sealed class ModuleBoundaryTests
{
    public static TheoryData<string, string> ModulePairs => Modules.OrderedPairs();

    public static TheoryData<string> ModuleNames => Modules.Names();

    [Theory]
    [MemberData(nameof(ModulePairs))]
    public void Module_does_not_reference_another_modules_implementation_assembly(string moduleName, string otherName)
    {
        var module = Modules.Get(moduleName);
        var other = Modules.Get(otherName);

        var referenced = module.Implementation.GetReferencedAssemblies().Select(a => a.Name);

        Assert.DoesNotContain(other.ImplementationAssemblyName, referenced);
    }

    [Theory]
    [MemberData(nameof(ModulePairs))]
    public void Module_types_do_not_depend_on_another_modules_internal_namespaces(string moduleName, string otherName)
    {
        var module = Modules.Get(moduleName);
        var other = Modules.Get(otherName);

        var result = Types.InAssembly(module.Implementation)
            .ShouldNot()
            .HaveDependencyOnAny(other.InternalNamespaces)
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result, $"{moduleName} must not use {otherName} internals"));
    }

    [Theory]
    [MemberData(nameof(ModuleNames))]
    public void Contracts_depend_only_on_the_base_class_library(string moduleName)
    {
        // Stricter than "no module implementation": no BuildingBlocks, EF Core or FastEndpoints either,
        // so a consumer of a contract can never be coupled to anything but the contract itself.
        var contracts = Modules.Get(moduleName).Contracts;

        Assert.Empty(contracts.NonBaseClassLibraryReferences());
    }

    [Theory]
    [MemberData(nameof(ModuleNames))]
    public void Module_implementation_publicly_exposes_only_its_entry_point(string moduleName)
    {
        // Everything else is internal, so even a stray project reference could not reach the internals.
        var module = Modules.Get(moduleName);

        var publicTypes = module.Implementation.GetExportedTypes().Select(t => t.FullName);

        Assert.Equal([$"{moduleName}.{moduleName}Module"], publicTypes);
    }

    private static string Describe(TestResult result, string rule) => result.Describe(rule);
}
