using BuildingBlocks.Results;

namespace Users.Domain;

/// <summary>
/// Every expected failure of the Users module, with a stable code clients can rely on (decision #22).
/// Includes failures detected by the application layer, because they are still domain language.
/// </summary>
internal static class UserErrors
{
    private const string UsernameField = "username";

    public static readonly Error UsernameRequired = Error.Validation(
        "users.username.required", "Username is required.", UsernameField);

    public static readonly Error UsernameInvalidLength = Error.Validation(
        "users.username.invalid_length",
        $"Username must be between {Username.MinLength} and {Username.MaxLength} characters.",
        UsernameField);

    public static readonly Error UsernameInvalidCharacters = Error.Validation(
        "users.username.invalid_characters",
        "Username may contain only letters, digits, '.', '_' and '-'.",
        UsernameField);

    public static readonly Error UsernameTaken = Error.Conflict(
        "users.username.taken", "Username is already taken.", UsernameField);

    public static readonly Error NotFound = Error.NotFound(
        "users.not_found", "User was not found.");
}
