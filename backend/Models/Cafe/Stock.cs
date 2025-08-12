#nullable enable
namespace UnnamHS_App_Backend.Models;

/// <summary>간단형 재고 (PK: MenuId, Date) → Fluent에서 복합키 구성</summary>
public class Stock
{
    public int MenuId { get; set; }
    public DateTime Date { get; set; } // 일 단위 관리면 자정 기준으로 저장

    public int Qty { get; set; }

    public MenuItem MenuItem { get; set; } = null!;
}
