namespace BuildingBlocks.Cqrs;

// Thin markers over the FastEndpoints command bus. The bus itself has no notion of "query",
// so these interfaces make the CQRS split explicit and let architecture tests enforce it.

/// <summary>Changes state. By convention returns at most the identifier of what it created.</summary>
public interface ICommand<TResult> : FastEndpoints.ICommand<TResult>
{
}

/// <summary>Reads state. Must never change it.</summary>
public interface IQuery<TResult> : FastEndpoints.ICommand<TResult>
{
}

public interface ICommandHandler<TCommand, TResult> : FastEndpoints.ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
}

public interface IQueryHandler<TQuery, TResult> : FastEndpoints.ICommandHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
}
