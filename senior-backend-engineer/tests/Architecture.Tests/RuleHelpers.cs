using System.Reflection;
using NetArchTest.Rules;

namespace Architecture.Tests;

internal static class RuleHelpers
{
    /// <summary>Referenced assemblies that are not part of the .NET base class library.</summary>
    public static IEnumerable<string> NonBaseClassLibraryReferences(this Assembly assembly) =>
        assembly.GetReferencedAssemblies()
            .Select(a => a.Name!)
            .Where(name => !(name is "netstandard" or "mscorlib" || name.StartsWith("System", StringComparison.Ordinal)));

    public static string Describe(this TestResult result, string rule) =>
        $"{rule}. Offending types: {string.Join(", ", result.FailingTypeNames ?? [])}";
}
