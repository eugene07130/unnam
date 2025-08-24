using System.ComponentModel.DataAnnotations;

namespace UnnamHS_App_Backend.DTOs;

/// <summary>
/// 학생코드 유효성 확인 요청 DTO
/// </summary>
public sealed class VerifyStudentRequest
{
    [Required]
    [StringLength(16, MinimumLength = 1)]
    public string StudentCode { get; set; } = null!;
}
