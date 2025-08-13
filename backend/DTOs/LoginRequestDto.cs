using System.ComponentModel.DataAnnotations;

namespace UnnamHS_App_Backend.DTOs;

public sealed class LoginRequestDto
{
    [Required, MinLength(3), MaxLength(32)]
    public string UserId { get; set; } = default!;

    [Required, MinLength(6), MaxLength(128)]
    public string Password { get; set; } = default!;
}
