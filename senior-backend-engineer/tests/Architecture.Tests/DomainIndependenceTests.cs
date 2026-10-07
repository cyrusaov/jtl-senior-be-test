using BuildingBlocks.Results;
using NetArchTest.Rules;

namespace Architecture.Tests;

/// <summary>The domain is plain C#: no web framework, no persistence framework, no application plumbing.</summary>
public sealed class DomainIndependenceTests
{
    // Frameworks and building blocks the domain must never see. BuildingBlocks.Results is deliberately absent:
    // it is the domain's dependency-free shared kernel.
    private static readonly string[] ForbiddenForDomain =
    [
        "FastEndpoints",
        "FluentValidation",
        "Microsoft.AspNetCore",
        "Microsoft.EntityFrameworkCore",
        "BuildingBlocks.Cqrs",
        "BuildingBlocks.Http",
    ];

    public static TheoryData<string> ModuleNames => Modules.Names();

    [Fact]
    public void Results_kernel_depends_only_on_the_base_class_library()
    {
        var kernel = typeof(Result<>).Assembly;

        Assert.Empty(kernel.NonBaseClassLibraryReferences());
    }

    [Theory]
    [MemberData(nameof(ModuleNames))]
    public void Domain_does_not_depend_on_any_framework(string moduleName)
    {
        var result = Types.InAssembly(Modules.Get(moduleName).Implementation)
            .That().ResideInNamespace($"{moduleName}.Domain")
            .ShouldNot().HaveDependencyOnAny(ForbiddenForDomain)
            .GetResult();

        Assert.True(result.IsSuccessful, result.Describe($"{moduleName}.Domain must stay framework-free"));
    }
}
