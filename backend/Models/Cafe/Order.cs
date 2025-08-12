#nullable enable
using System.ComponentModel.DataAnnotations;

namespace UnnamHS_App_Backend.Models;

/// <summary>주문 헤더(4NF: 라인/옵션과 분리)</summary>
public class Order
{
    [Key]
    public long OrderId { get; set; }

    [Required, MaxLength(16)]
    public string StudentCode { get; set; } = null!;

    public DateTime OrderedAt { get; set; } = DateTime.UtcNow;

    /// <summary>"pending","paid","ready","done","canceled"</summary>
    [MaxLength(16)]
    public string Status { get; set; } = "pending";

    public bool ToGo { get; set; }
    public string? Request { get; set; }

    /// <summary>주문 총액(스냅샷)</summary>
    public int TotalPrice { get; set; }

    public Student Student { get; set; } = null!;
    public ICollection<OrderLine> Lines { get; set; } = new List<OrderLine>();
}
