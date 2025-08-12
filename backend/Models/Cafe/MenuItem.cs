#nullable enable
using System.ComponentModel.DataAnnotations;

namespace UnnamHS_App_Backend.Models;

/// <summary>카페 메뉴</summary>
public class MenuItem
{
    [Key]
    public int MenuId { get; set; }

    [Required, MaxLength(64)]
    public string Name { get; set; } = null!; // UNIQUE (Fluent)

    public int BasePrice { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<OrderLine> Lines { get; set; } = new List<OrderLine>();
    public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
