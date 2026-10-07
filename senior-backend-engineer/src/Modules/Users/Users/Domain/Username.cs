using System.Text.RegularExpressions;
using BuildingBlocks.Results;

namespace Users.Domain;

/// <summary>
/// A validated username (decision #1): trimmed, 3–32 characters of <c>[a-zA-Z0-9._-]</c>.
/// Kept as entered for display; <see cref="NormalizedValue"/> is the case-insensitive identity
/// used for equality and uniqueness.
/// </summary>
internal sealed partial class Username : IEquatable<Username>
{
    public const int MinLength = 3;
    public const int MaxLength = 32;

    private Username(string value)
    {
        Value = value;
        NormalizedValue = value.ToUpperInvariant();
    }

    public string Value { get; private set; }

    public string NormalizedValue { get; private set; }

    public static Result<Username> Create(string? input)
    {
        var value = input?.Trim();

        if (string.IsNullOrEmpty(value))
            return UserErrors.UsernameRequired;

        if (value.Length is < MinLength or > MaxLength)
            return UserErrors.UsernameInvalidLength;

        if (!AllowedCharacters().IsMatch(value))
            return UserErrors.UsernameInvalidCharacters;

        return new Username(value);
    }

    public bool Equals(Username? other) =>
        other is not null && NormalizedValue == other.NormalizedValue;

    public override bool Equals(object? obj) => Equals(obj as Username);

    public override int GetHashCode() => NormalizedValue.GetHashCode(StringComparison.Ordinal);

    public override string ToString() => Value;

    [GeneratedRegex("^[a-zA-Z0-9._-]+$")]
    private static partial Regex AllowedCharacters();
}
