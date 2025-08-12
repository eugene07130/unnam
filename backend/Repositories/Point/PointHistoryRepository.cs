using Microsoft.EntityFrameworkCore;
using UnnamHS_App_Backend.Data;
using UnnamHS_App_Backend.Models;

namespace UnnamHS_App_Backend.Repositories;

public class PointHistoryRepository : IPointHistoryRepository
{
    private readonly AppDbContext _db;
    public PointHistoryRepository(AppDbContext db) => _db = db;

    public Task<int> GetTotalAsync(string studentCode) =>
        _db.PointHistories
           .Where(p => p.StudentCode == studentCode)
           .SumAsync(p => (int?)p.Points ?? 0);

    public async Task AddAsync(PointHistory entry)
    {
        // CreatedAt은 DB(UTC) 기본값 쓰거나, 여기서 DateTime.UtcNow로 지정
        if (entry.CreatedAt == default)
            entry.CreatedAt = DateTime.UtcNow;

        await _db.PointHistories.AddAsync(entry);
    }

    public async Task<IReadOnlyList<PointHistory>> GetRecentAsync(string studentCode, int take = 50)
    {
        var list = await _db.PointHistories
            .Where(p => p.StudentCode == studentCode)
            .OrderByDescending(p => p.CreatedAt)
            .Take(take)
            .ToListAsync();

        return list;
    }
}
