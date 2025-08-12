using Microsoft.EntityFrameworkCore;
using UnnamHS_App_Backend.Models;
using UnnamHS_App_Backend.Repositories;
using UnnamHS_App_Backend.Data;

namespace UnnamHS_App_Backend.Services;

public class PointsService : IPointsService
{
    private readonly IPointHistoryRepository _repo;
    private readonly IStudentRepository _students;
    private readonly AppDbContext _db;

    public PointsService(IPointHistoryRepository repo, IStudentRepository students, AppDbContext db)
    {
        _repo = repo; _students = students; _db = db;
    }

    public Task<int> GetTotalAsync(string studentCode) => _repo.GetTotalAsync(studentCode);

    public async Task AddAsync(string studentCode, int points, string source)
    {
        if (points == 0) return;
        if (!await _students.ExistsAsync(studentCode))
            throw new InvalidOperationException("학생코드가 존재하지 않습니다.");

        await _repo.AddAsync(new PointHistory {
            StudentCode = studentCode,
            Source = source,
            Points = points,
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
    }

    public async Task<bool> TryUseAsync(string studentCode, int points, string source)
    {
        if (points <= 0) return false;
        var total = await _repo.GetTotalAsync(studentCode);
        if (total < points) return false;

        await _repo.AddAsync(new PointHistory {
            StudentCode = studentCode,
            Source = source,
            Points = -points,
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        return true;
    }
}
