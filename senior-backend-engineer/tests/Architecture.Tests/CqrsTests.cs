using System.Reflection;
using BuildingBlocks.Cqrs;
using BuildingBlocks.Results;
using NetArchTest.Rules;

namespace Architecture.Tests;

/// <summary>
/// The command/query split. The FastEndpoints bus has no notion of "query", so the ICommand/IQuery markers
/// carry the distinction and these tests enforce it.
/// </summary>
public sealed class CqrsTests
{
    public static TheoryData<string> ModuleNames => Modules.Names();

    [Theory]
    [MemberData(nameof(ModuleNames))]
    public void Commands_and_queries_are_named_for_what_they_are_and_live_in_Application(string moduleName)
    {
        var types = Modules.Get(moduleName).Implementation.GetTypes();
        var commands = types.Where(t => Implements(t, typeof(ICommand<>))).ToList();
        var queries = types.Where(t => Implements(t, typeof(IQuery<>))).ToList();

        Assert.NotEmpty(commands);
        Assert.NotEmpty(queries);

        var violations = new List<string>();
        violations.AddRange(commands.Where(t => !t.Name.EndsWith("Command")).Select(t => $"{t} implements ICommand<> but is not named *Command"));
        violations.AddRange(queries.Where(t => !t.Name.EndsWith("Query")).Select(t => $"{t} implements IQuery<> but is not named *Query"));
        violations.AddRange(types.Where(t => t.Name.EndsWith("Command") && !commands.Contains(t)).Select(t => $"{t} is named *Command but does not implement ICommand<>"));
        violations.AddRange(types.Where(t => t.Name.EndsWith("Query") && !queries.Contains(t)).Select(t => $"{t} is named *Query but does not implement IQuery<>"));
        violations.AddRange(commands.Concat(queries)
            .Where(t => t.Namespace?.StartsWith($"{moduleName}.Application.") != true)
            .Select(t => $"{t} must live in {moduleName}.Application"));

        Assert.Empty(violations);
    }

    [Theory]
    [MemberData(nameof(ModuleNames))]
    public void Commands_return_at_most_an_id(string moduleName)
    {
        var offending = Modules.Get(moduleName).Implementation.GetTypes()
            .Select(t => (Type: t, Result: GenericArgument(t, typeof(ICommand<>))))
            .Where(c => c.Result is not null && c.Result != typeof(Result<Guid>))
            .Select(c => $"{c.Type} returns {c.Result} instead of Result<Guid>");

        Assert.Empty(offending);
    }

    [Theory]
    [MemberData(nameof(ModuleNames))]
    public void Query_handlers_never_touch_the_write_side(string moduleName)
    {
        var assembly = Modules.Get(moduleName).Implementation;

        // The write side of a module is its aggregate repositories (Domain ports named *Repository).
        var repositories = assembly.GetTypes()
            .Where(t => t.IsInterface && t.Namespace == $"{moduleName}.Domain" && t.Name.EndsWith("Repository"))
            .Select(t => t.FullName!)
            .ToArray();
        Assert.NotEmpty(repositories);

        var queryHandlers = Types.InAssembly(assembly).That().HaveDependencyOn(typeof(IQueryHandler<,>).FullName!);
        Assert.NotEmpty(queryHandlers.GetTypes());

        var result = queryHandlers.ShouldNot().HaveDependencyOnAny(repositories).GetResult();

        Assert.True(result.IsSuccessful, result.Describe($"Query handlers must not use {string.Join(", ", repositories)}"));
    }

    private static bool Implements(Type type, Type openGenericInterface) =>
        GenericArgument(type, openGenericInterface) is not null;

    private static Type? GenericArgument(Type type, Type openGenericInterface) =>
        type.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericInterface)?
            .GetGenericArguments()[0];
}
