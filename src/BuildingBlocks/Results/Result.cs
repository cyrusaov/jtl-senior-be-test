using System.Diagnostics.CodeAnalysis;

namespace BuildingBlocks.Results;

/// <summary>Either a value or an <see cref="Results.Error"/>. Never both, never neither.</summary>
public sealed class Result<T>
{
    private readonly T? _value;

    private Result(T value) => _value = value;

    private Result(Error error) => Error = error;

    public Error? Error { get; }

    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Error is null;

    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailure => !IsSuccess;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException($"Cannot read the value of a failed result ({Error.Code}).");

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(Error error) => new(error);

    public static implicit operator Result<T>(T value) => Success(value);

    public static implicit operator Result<T>(Error error) => Failure(error);
}
