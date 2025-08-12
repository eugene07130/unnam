#nullable enable
using System.ComponentModel.DataAnnotations;

namespace UnnamHS_App_Backend.Models;

/// <summary>포인트 적립/차감 이력(+/-)</summary>
public class PointHistory
{
    [Key]
    public long Id { get; set; }

    [Required, MaxLength(16)]
    public string StudentCode { get; set; } = null!;

    [Required]
    public string Source { get; set; } = null!; // "recycle","library","event" …

    public int Points { get; set; } // +/-

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Student Student { get; set; } = null!;
}
