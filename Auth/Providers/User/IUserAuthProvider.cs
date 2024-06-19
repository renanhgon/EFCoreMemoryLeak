namespace Auth.Providers.User;

public interface IUserAuthProvider
{
    public string Email { get; }
    bool IsAuthenticated { get; }
    public string Role { get; }
    public Guid UserId { get; }
    public string Username { get; }
}