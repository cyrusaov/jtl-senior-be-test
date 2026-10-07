using BuildingBlocks.Results;

namespace Users.Domain;

/// <summary>Every expected failure of the Users domain, with a stable code clients can rely on.</summary>
internal static class UserErrors
{
    private const string UsernameField = "username";

    public static readonly Error UsernameRequired = Error.Validation(
        UsernameField, "users.username.required", "Username is required.");

    public static readonly Error UsernameInvalidLength = Error.Validation(
        UsernameField, "users.username.invalid_length",
        $"Username must be between {Username.MinLength} and {Username.MaxLength} characters.");

    public static readonly Error UsernameInvalidCharacters = Error.Validation(
        UsernameField, "users.username.invalid_characters",
        "Username may contain only letters, digits, '.', '_' and '-'.");

    public static readonly Error UsernameTaken = Error.Conflict(
        "users.username.taken", "Username is already taken.", UsernameField);
}
