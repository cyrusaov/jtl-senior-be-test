namespace Users.Domain;

/// <summary>Write-side access to <see cref="User"/> aggregates. Reads go through query handlers (decision #14).</summary>
internal interface IUserRepository
{
    Task<bool> ExistsByUsernameAsync(Username username, CancellationToken ct);

    Task AddAsync(User user, CancellationToken ct);
}
