using System.ComponentModel.DataAnnotations;

namespace UnnamHS_App_Backend.DTOs;

// == 요청 DTO ==

public sealed class RegisterUserDto
{
    // 로그인 ID
    [Required, StringLength(64, MinimumLength = 3)]
    public string UserId { get; set; } = null!;

    // 평문 비밀번호(서비스에서 해시)
    [Required, StringLength(128, MinimumLength = 8)]
    public string Password { get; set; } = null!;

    // 표시 이름
    [Required, StringLength(64, MinimumLength = 1)]
    public string Name { get; set; } = null!;

    // 연동할 학생코드
    [Required, StringLength(16, MinimumLength = 1)]
    public string StudentCode { get; set; } = null!;
}

// == 응답 DTO ==

public sealed class RegisterResult
{
    [Required, StringLength(64, MinimumLength = 1)]
    public string UserId { get; init; } = null!;

    [Required, StringLength(64, MinimumLength = 1)]
    public string Name { get; init; } = null!;

    [Required, StringLength(16, MinimumLength = 1)]
    public string StudentCode { get; init; } = null!;

    // "student" 등
    [Required, StringLength(16, MinimumLength = 1)]
    public string Role { get; init; } = "student";
}

public sealed class UserIdAvailabilityDto
{
    [Required, StringLength(64, MinimumLength = 1)]
    public string UserId { get; init; } = null!;

    public bool IsAvailable { get; init; }
}
