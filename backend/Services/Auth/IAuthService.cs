namespace UnnamHS_App_Backend.Services;

public interface IAuthService
{
    Task<(bool ok, string? error)> RegisterAsync(string userId, string rawPassword, string name, string studentCode, string role);
    Task<(bool ok, string? token, string? error)> LoginAsync(string userId, string rawPassword);
}
