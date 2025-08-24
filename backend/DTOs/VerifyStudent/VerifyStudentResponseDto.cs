using System.ComponentModel.DataAnnotations;

namespace UnnamHS_App_Backend.DTOs;
// 요청: 학생코드 검증

/// <summary>
/// 간단 학생 요약 정보 (API 응답용)
/// </summary>
public sealed class StudentSummaryDto
{
    [Required]
    [StringLength(16, MinimumLength = 1)]
    public string StudentCode { get; init; } = null!;

    [Required]
    [StringLength(64, MinimumLength = 1)]
    public string Name { get; init; } = null!;
}

/// <summary>
/// 학생코드 검증 결과 DTO
/// - Exists: students 테이블에 존재 여부
/// - IsTaken: users.StudentCode로 이미 연결되어 사용 중인지
/// - IsUsable: 가입/연동에 바로 사용 가능한지 (Exists && !IsTaken)
/// </summary>
public sealed class VerifyStudentResponse
{
    [Required]
    [StringLength(16, MinimumLength = 1)]
    public string StudentCode { get; init; } = null!;

    public bool Exists { get; init; }
    public bool IsTaken { get; init; }
    public bool IsUsable { get; init; }

    /// <summary>존재할 경우 학생 기본 정보(코드/이름) 제공</summary>
    public StudentSummaryDto? Student { get; init; }
}
