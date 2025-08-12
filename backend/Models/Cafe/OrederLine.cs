#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UnnamHS_App_Backend.Models;

/// <summary>주문 품목 라인 (PK: OrderId, LineNo) → Fluent에서 복합키 구성</summary>
public class OrderLine
{
    /// <summary>복합키 part 1 (Fluent에서 HasKey)</summary>
    public long OrderId { get; set; }

    /// <summary>복합키 part 2 (1..n)</summary>
    public int LineNo { get; set; }

    public int MenuId { get; set; }

    public int Qty { get; set; }
    public int UnitPrice { get; set; } // 주문 시점 단가
    public int LinePrice { get; set; } // (단가+옵션합) * Qty

    public Order Order { get; set; } = null!;
    public MenuItem MenuItem { get; set; } = null!;
    public ICollection<OrderLineOption> Options { get; set; } = new List<OrderLineOption>();
}
