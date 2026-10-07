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
    public void Contracts_do_not_reference_any_module_implementation(string moduleName)
    {
        var contracts = Modules.Get(moduleName).Contracts;

        var referenced = contracts.GetReferencedAssemblies().Select(a => a.Name).ToHashSet();
        var offending = Modules.All.Select(m => m.ImplementationAssemblyName).Where(referenced.Contains);

        Assert.Empty(offending);
    }

    private static string Describe(TestResult result, string rule) =>
        $"{rule}. Offending types: {string.Join(", ", result.FailingTypeNames ?? [])}";
}
