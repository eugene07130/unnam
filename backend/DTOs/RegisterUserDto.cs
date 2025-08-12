namespace UnnamHS_App_Backend.DTOs;

public sealed class RegisterUserDto
{
    // users.Id
    public string UserId { get; init; } = string.Empty;

    // 해시 대상
    public string Password { get; init; } = string.Empty;

    // users.StudentCode (FK→students.StudentCode, UNIQUE)
    public string StudentCode { get; init; } = string.Empty;
}

