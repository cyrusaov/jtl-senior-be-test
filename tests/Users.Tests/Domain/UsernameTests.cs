using Users.Domain;

namespace Users.Tests.Domain;

public sealed class UsernameTests
{
    [Theory]
    [InlineData("abc")]
    [InlineData("Alice")]
    [InlineData("john.doe_42-x")]
    [InlineData("abcdefghijklmnopqrstuvwxyz012345")] // exactly 32
    public void Accepts_valid_usernames(string input)
    {
        var result = Username.Create(input);

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(input);
    }

    [Fact]
    public void Trims_surrounding_whitespace_and_keeps_original_casing()
    {
        var username = Username.Create("  Alice_01 ").Value;

        username.Value.Should().Be("Alice_01");
        username.NormalizedValue.Should().Be("ALICE_01");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Rejects_missing_username(string? input) =>
        Username.Create(input).Error.Should().Be(UserErrors.UsernameRequired);

    [Theory]
    [InlineData("ab")]
    [InlineData("  ab  ")] // length is checked after trimming
    [InlineData("abcdefghijklmnopqrstuvwxyz0123456")] // 33
    public void Rejects_username_with_invalid_length(string input) =>
        Username.Create(input).Error.Should().Be(UserErrors.UsernameInvalidLength);

    [Theory]
    [InlineData("al ice")]
    [InlineData("alice!")]
    [InlineData("ålice")]
    [InlineData("al@ice")]
    public void Rejects_username_with_invalid_characters(string input) =>
        Username.Create(input).Error.Should().Be(UserErrors.UsernameInvalidCharacters);

    [Fact]
    public void Equality_is_case_insensitive()
    {
        var upper = Username.Create("Alice").Value;
        var lower = Username.Create("alice").Value;

        upper.Should().Be(lower);
        upper.GetHashCode().Should().Be(lower.GetHashCode());
    }
}
