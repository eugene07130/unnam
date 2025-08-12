using Microsoft.EntityFrameworkCore;
using UnnamHS_App_Backend.Data;
using UnnamHS_App_Backend.Models;
using System.Threading.Tasks;

namespace UnnamHS_App_Backend.Services;
public class StudentVerifyService : IStudentVerifyService
{
    private readonly AppDbContext _db;
    public StudentVerifyService(AppDbContext db) => _db = db;

    public async Task<bool> IsUsableAsync(string studentCode)
    {
        var exists = await _db.Students.AnyAsync(s => s.StudentCode == studentCode);
        if (!exists) return false;

        var taken = await _db.Users.AnyAsync(u => u.StudentCode == studentCode);
        return !taken;
    }

    public Task<Student?> GetByCodeAsync(string studentCode) =>
        _db.Students.FirstOrDefaultAsync(s => s.StudentCode == studentCode);
}

