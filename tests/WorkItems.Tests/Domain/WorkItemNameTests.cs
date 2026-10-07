using WorkItems.Domain;

namespace WorkItems.Tests.Domain;

public sealed class WorkItemNameTests
{
    [Theory]
    [InlineData("A")]
    [InlineData("Write the README")]
    public void Accepts_valid_names(string input) =>
        WorkItemName.Create(input).Value.Value.Should().Be(input);

    [Fact]
    public void Accepts_exactly_max_length() =>
        WorkItemName.Create(new string('x', WorkItemName.MaxLength)).IsSuccess.Should().BeTrue();

    [Fact]
    public void Trims_surrounding_whitespace() =>
        WorkItemName.Create("  Fix bug  ").Value.Value.Should().Be("Fix bug");

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Rejects_missing_name(string? input) =>
        WorkItemName.Create(input).Error.Should().Be(WorkItemErrors.NameRequired);

    [Fact]
    public void Rejects_name_longer_than_max_length() =>
        WorkItemName.Create(new string('x', WorkItemName.MaxLength + 1)).Error.Should().Be(WorkItemErrors.NameTooLong);
}
