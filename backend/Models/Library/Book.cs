#nullable enable
using System.ComponentModel.DataAnnotations;

namespace UnnamHS_App_Backend.Models;

/// <summary>도서 메타 정보</summary>
public class Book
{
    [Key]
    [MaxLength(32)]
    public string BookCode { get; set; } = null!;

    [Required, MaxLength(256)]
    public string Title { get; set; } = null!;

    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public int? Year { get; set; }
    public string? Kdc { get; set; }
    public string? Isbn { get; set; }
    public string? MajorClass { get; set; }
    public string? SubClass { get; set; }

    public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
    public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
}
