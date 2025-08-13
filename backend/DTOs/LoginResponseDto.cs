using System.ComponentModel.DataAnnotations;

namespace UnnamHS_App_Backend.DTOs;

public sealed class LoginResponseDto
{
    [Required]
    public string Token { get; set; } = default!;

    [Required]
    public string UserId { get; set; } = default!;

    [Required]
    public string Role { get; set; } = default!;

    // JWT에 포함되면 프런트에서 활용 가능
    public string? StudentCode { get; set; }
}
