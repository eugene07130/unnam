using Microsoft.EntityFrameworkCore;
using UnnamHS_App_Backend.Data;
using UnnamHS_App_Backend.Models;

namespace UnnamHS_App_Backend.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public Task<User?> GetByIdAsync(string userId) =>
        _db.Users
           .Include(u => u.Student)        // 필요시: 학생 프로필도 함께
           .FirstOrDefaultAsync(u => u.Id == userId);

    public Task<User?> GetByStudentCodeAsync(string studentCode) =>
        _db.Users.FirstOrDefaultAsync(u => u.StudentCode == studentCode);

    public Task<bool> ExistsAsync(string userId) =>
        _db.Users.AnyAsync(u => u.Id == userId);

    public async Task AddAsync(User user)
    {
        await _db.Users.AddAsync(user);
    }

    public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();
}
