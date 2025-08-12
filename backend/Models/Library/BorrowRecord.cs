#nullable enable
using System.ComponentModel.DataAnnotations;

namespace UnnamHS_App_Backend.Models;

/// <summary>도서 대출 기록</summary>
public class BorrowRecord
{
    [Key]
    public long Id { get; set; }

    [Required, MaxLength(64)]
    public string UserId { get; set; } = null!;

    [Required, MaxLength(32)]
    public string BookCode { get; set; } = null!;

    public DateTime BorrowedAt { get; set; }
    public DateTime DueDate { get; set; }
    public int ExtensionCount { get; set; } = 0;
    public bool IsReturned { get; set; } = false;

    public User User { get; set; } = null!;
    public Book Book { get; set; } = null!;
}
