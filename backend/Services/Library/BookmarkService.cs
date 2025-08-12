using Microsoft.EntityFrameworkCore;
using UnnamHS_App_Backend.Models;
using UnnamHS_App_Backend.Data;

namespace UnnamHS_App_Backend.Services;

public class BookmarkService : IBookmarkService
{
    private readonly AppDbContext _db;
    public BookmarkService(AppDbContext db) => _db = db;

    public async Task<bool> AddAsync(string userId, string bookCode)
    {
        if (!await _db.Users.AnyAsync(u => u.Id == userId)) return false;
        if (!await _db.Books.AnyAsync(b => b.BookCode == bookCode)) return false;

        // UNIQUE(UserId, BookCode)
        var exists = await _db.Bookmarks.AnyAsync(x => x.UserId == userId && x.BookCode == bookCode);
        if (exists) return true;

        _db.Bookmarks.Add(new Bookmark { UserId = userId, BookCode = bookCode });
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveAsync(string userId, string bookCode)
    {
        var row = await _db.Bookmarks.FirstOrDefaultAsync(x => x.UserId == userId && x.BookCode == bookCode);
        if (row is null) return false;
        _db.Bookmarks.Remove(row);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<IReadOnlyList<Bookmark>> ListAsync(string userId)
        => _db.Bookmarks.Where(x => x.UserId == userId).ToListAsync()
            .ContinueWith(t => (IReadOnlyList<Bookmark>)t.Result);
}
