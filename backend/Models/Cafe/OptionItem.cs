#nullable enable
using System.ComponentModel.DataAnnotations;

namespace UnnamHS_App_Backend.Models;

/// <summary>옵션(샷추가, 사이즈업 등)</summary>
public class OptionItem
{
    [Key]
    public int OptionId { get; set; }

    [Required, MaxLength(64)]
    public string Name { get; set; } = null!;

    public int PriceDelta { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<OrderLineOption> OrderLineOptions { get; set; } = new List<OrderLineOption>();
}
