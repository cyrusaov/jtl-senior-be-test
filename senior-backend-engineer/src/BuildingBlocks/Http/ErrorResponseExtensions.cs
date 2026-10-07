using BuildingBlocks.Results;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Http;

public static class ErrorResponseExtensions
{
    private const string GeneralErrorsField = "generalErrors";

    /// <summary>
    /// Sends an <see cref="Error"/> through FastEndpoints' error pipeline, so business failures and
    /// request-binding failures share one RFC 7807 problem-details shape.
    /// </summary>
    public static Task SendErrorAsync<TRequest, TResponse>(
        this ResponseSender<TRequest, TResponse> send, Error error, CancellationToken ct)
        where TRequest : notnull
    {
        send.ValidationFailures.Add(
            new ValidationFailure(error.Field ?? GeneralErrorsField, error.Message) { ErrorCode = error.Code });

        return send.ErrorsAsync(ToStatusCode(error.Kind), ct);
    }

    public static int ToStatusCode(this ErrorKind kind) => kind switch
    {
        ErrorKind.Validation => StatusCodes.Status400BadRequest,
        ErrorKind.NotFound => StatusCodes.Status404NotFound,
        ErrorKind.Conflict => StatusCodes.Status409Conflict,
        ErrorKind.Unprocessable => StatusCodes.Status422UnprocessableEntity,
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
    };
}
