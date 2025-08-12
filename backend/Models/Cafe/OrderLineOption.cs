#nullable enable
namespace UnnamHS_App_Backend.Models;

/// <summary>라인-옵션 교차 테이블 (PK: OrderId, LineNo, OptionId) → Fluent에서 복합키 구성</summary>
public class OrderLineOption
{
    public long OrderId { get; set; }
    public int LineNo { get; set; }
    public int OptionId { get; set; }
    public OrderLine OrderLine { get; set; } = null!;
    public OptionItem OptionItem { get; set; } = null!;
}
