namespace UnnamHS_App_Backend.DTOs;

public class LoginResponseDto
{
    public string? Token { get; set; }
    public string? Role { get; set; }
    public string? Username { get; set; }
}