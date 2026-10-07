namespace Users.Contracts;

/// <summary>
/// What the Users module offers to other modules, and nothing more (decision #23).
/// Uses a plain <see cref="Guid"/> so no Users domain type ever crosses the module boundary.
/// </summary>
public interface IUserDirectory
{
    Task<bool> ExistsAsync(Guid userId, CancellationToken ct);
}
