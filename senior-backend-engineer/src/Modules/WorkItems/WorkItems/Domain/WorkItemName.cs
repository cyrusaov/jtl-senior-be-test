using BuildingBlocks.Results;

namespace WorkItems.Domain;

/// <summary>A work item's name: trimmed, non-blank, at most <see cref="MaxLength"/> characters.</summary>
internal sealed record WorkItemName
{
    public const int MaxLength = 200;

    private WorkItemName(string value) => Value = value;

    public string Value { get; }

    public static Result<WorkItemName> Create(string? input)
    {
        var value = input?.Trim();

        if (string.IsNullOrEmpty(value))
            return WorkItemErrors.NameRequired;

        if (value.Length > MaxLength)
            return WorkItemErrors.NameTooLong;

        return new WorkItemName(value);
    }

    public override string ToString() => Value;
}
