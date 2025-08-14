namespace UnnamHS_App_Backend.DTOs;

/// <summary>
/// 포인트 요청 DTO
/// </summary>
public class PointRequestDto
{
    /// <summary>
    /// 변동 포인트 양 (양수: 적립, 음수: 사용)
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// 포인트 변동 사유
    /// </summary>
    public string? Source { get; set; }
}

