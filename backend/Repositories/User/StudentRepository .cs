using Microsoft.EntityFrameworkCore;
using UnnamHS_App_Backend.Data;
using UnnamHS_App_Backend.Models;

namespace UnnamHS_App_Backend.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _db;
    public StudentRepository(AppDbContext db) => _db = db;

    public Task<bool> ExistsAsync(string studentCode) =>
        _db.Students.AnyAsync(s => s.StudentCode == studentCode);

    public Task<Student?> GetAsync(string studentCode) =>
        _db.Students.FirstOrDefaultAsync(s => s.StudentCode == studentCode);
}
