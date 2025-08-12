#nullable enable
using System.ComponentModel.DataAnnotations;

namespace UnnamHS_App_Backend.Models;

/// <summary>독서 북마크 (유저-책당 1개: Unique(UserId, BookCode))</summary>
public class Bookmark
{
    [Key]
    public long Id { get; set; }

    [Required, MaxLength(64)]
    public string UserId { get; set; } = null!;

    [Required, MaxLength(32)]
    public string BookCode { get; set; } = null!;

    public int? PageCurrent { get; set; }
    public int? PageTotal { get; set; }
    public string? Status { get; set; }   // "reading","paused","finished"…
    public string? Note { get; set; }
    public int? GoalPage { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public DateTime? LastReadAt { get; set; }

    public User User { get; set; } = null!;
    public Book Book { get; set; } = null!;
}
