using Microsoft.EntityFrameworkCore;
using UnnamHS_App_Backend.Models;
using UnnamHS_App_Backend.Repositories;
using UnnamHS_App_Backend.Data;


namespace UnnamHS_App_Backend.Services;

public class BorrowService : IBorrowService
{
    private readonly AppDbContext _db;
    public BorrowService(AppDbContext db) => _db = db;

    public async Task<long> BorrowAsync(string userId, string bookCode, DateTime borrowedAtUtc)
    {
        // 책/유저 존재 체크
        var user = await _db.Users.FindAsync(userId) ?? throw new InvalidOperationException("사용자 없음");
        var book = await _db.Books.FindAsync(bookCode) ?? throw new InvalidOperationException("도서 없음");

        var rec = new BorrowRecord
        {
            UserId = userId,
            BookCode = bookCode,
            BorrowedAt = borrowedAtUtc,
            DueDate = borrowedAtUtc.AddDays(14),
            ExtensionCount = 0,
            IsReturned = false
        };
        _db.BorrowRecords.Add(rec);
        await _db.SaveChangesAsync();
        return rec.Id;
    }

    public async Task<bool> ReturnAsync(long recordId)
    {
        var rec = await _db.BorrowRecords.FirstOrDefaultAsync(x => x.Id == recordId);
        if (rec is null || rec.IsReturned) return false;
        rec.IsReturned = true;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExtendAsync(long recordId)
    {
        var rec = await _db.BorrowRecords.FirstOrDefaultAsync(x => x.Id == recordId);
        if (rec is null || rec.IsReturned) return false;
        if (rec.ExtensionCount >= 2) return false;

        rec.DueDate = rec.DueDate.AddDays(7);
        rec.ExtensionCount += 1;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<BorrowRecord>> GetOpenByUserAsync(string userId)
    {
        return await _db.BorrowRecords
            .Where(x => x.UserId == userId && !x.IsReturned)
            .OrderBy(x => x.DueDate)
            .ToListAsync();
    }
}
