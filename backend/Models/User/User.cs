#nullable enable
using System.ComponentModel.DataAnnotations;

namespace UnnamHS_App_Backend.Models;

/// <summary>로그인 사용자</summary>
public class User
{
    /// <summary>로그인 아이디 (PK)</summary>
    [Key]
    [MaxLength(64)]
    public string Id { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    [MaxLength(64)]
    public string Name { get; set; } = null!;

    /// <summary>"student" | "kiosk" | "admin" …</summary>
    [MaxLength(16)]
    public string Role { get; set; } = "student";

    /// <summary>학생 FK (UNIQUE: 한 학생 1계정 정책)</summary>
    [MaxLength(16)]
    public string StudentCode { get; set; } = null!;

    public Student Student { get; set; } = null!;

    public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
    public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
}
