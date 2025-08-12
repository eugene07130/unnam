using System.ComponentModel.DataAnnotations;

namespace UnnamHS_App_Backend.DTOs;

/// <summary>
/// 포인트 증감 요청 DTO
/// </summary>
public class PointRequestDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Amount는 1 이상이어야 합니다.")]
    public int Amount { get; init; }
}
