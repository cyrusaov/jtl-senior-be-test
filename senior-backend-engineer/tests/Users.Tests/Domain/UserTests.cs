using Users.Domain;

namespace Users.Tests.Domain;

public sealed class UserTests
{
    [Fact]
    public void Create_assigns_a_new_identity_and_keeps_the_username()
    {
        var username = Username.Create("alice").Value;

        var user = User.Create(username);

        user.Id.Value.Should().NotBeEmpty();
        user.Username.Should().Be(username);
    }

    [Fact]
    public void Each_created_user_gets_a_distinct_identity()
    {
        var username = Username.Create("alice").Value;

        User.Create(username).Id.Should().NotBe(User.Create(username).Id);
    }

    [Fact]
    public void Exposes_no_public_setters()
    {
        var publicSetters = typeof(User).GetProperties()
            .Where(p => p.SetMethod?.IsPublic == true)
            .Select(p => p.Name);

        publicSetters.Should().BeEmpty();
    }
}
