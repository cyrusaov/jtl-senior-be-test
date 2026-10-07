namespace Users.Domain;

/// <summary>Aggregate root of the Users module.</summary>
internal sealed class User
{
    // Required by EF Core to materialize the aggregate; not usable by application code.
    private User()
    {
        Username = null!;
    }

    private User(UserId id, Username username)
    {
        Id = id;
        Username = username;
    }

    public UserId Id { get; private set; }

    public Username Username { get; private set; }

    /// <summary>
    /// Creates a new user. Uniqueness of <paramref name="username"/> spans all users, so it is checked
    /// by the caller (CreateUserHandler) against the repository, not by the aggregate (decision #12).
    /// </summary>
    public static User Create(Username username) => new(UserId.New(), username);
}
