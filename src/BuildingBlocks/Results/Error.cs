namespace BuildingBlocks.Results;

/// <summary>
/// An expected business failure (as opposed to an exception, which signals a bug or an outage).
/// <see cref="Kind"/> decides the HTTP status; <see cref="Code"/> is a stable, machine-readable identifier.
/// </summary>
public sealed record Error(string Code, string Message, ErrorKind Kind, string? Field = null)
{
    // Every factory takes (code, message, field): one argument order, no swapped-string bugs.
    public static Error Validation(string code, string message, string field) =>
        new(code, message, ErrorKind.Validation, field);

    public static Error NotFound(string code, string message, string? field = null) =>
        new(code, message, ErrorKind.NotFound, field);

    public static Error Conflict(string code, string message, string? field = null) =>
        new(code, message, ErrorKind.Conflict, field);

    public static Error Unprocessable(string code, string message, string? field = null) =>
        new(code, message, ErrorKind.Unprocessable, field);
}

public enum ErrorKind
{
    Validation,
    NotFound,
    Conflict,
    Unprocessable,
}
