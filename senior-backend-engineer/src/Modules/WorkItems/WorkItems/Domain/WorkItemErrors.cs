using BuildingBlocks.Results;

namespace WorkItems.Domain;

/// <summary>
/// Every expected failure of the WorkItems module, with a stable code clients can rely on (decision #22).
/// Includes failures detected by the application layer, because they are still domain language.
/// </summary>
internal static class WorkItemErrors
{
    private const string NameField = "name";
    private const string AssigneeField = "assigneeId";

    public static readonly Error NameRequired = Error.Validation(
        "workitems.name.required", "Name is required.", NameField);

    public static readonly Error NameTooLong = Error.Validation(
        "workitems.name.too_long", $"Name must be at most {WorkItemName.MaxLength} characters.", NameField);

    public static readonly Error AssigneeRequired = Error.Validation(
        "workitems.assignee.required", "Assignee is required.", AssigneeField);

    public static readonly Error AssigneeNotFound = Error.Unprocessable(
        "workitems.assignee.not_found", "Assignee does not exist.", AssigneeField);
}
