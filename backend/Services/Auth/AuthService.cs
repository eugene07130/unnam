using Microsoft.EntityFrameworkCore;
using UnnamHS_App_Backend.Models;
using UnnamHS_App_Backend.Repositories;
using UnnamHS_App_Backend.Data;

namespace UnnamHS_App_Backend.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IStudentRepository _students;
    private readonly IJwtTokenFactory _jwt; 
    private readonly AppDbContext _db;

    public AuthService(IUserRepository users, IStudentRepository students, IJwtTokenFactory jwt, AppDbContext db)
    {
        _users = users;
        _students = students;
        _jwt = jwt;
        _db = db;
    }

    public async Task<(bool ok, string? error)> RegisterAsync(string userId, string rawPassword, string name, string studentCode, string role)
    {
        if (!await _students.ExistsAsync(studentCode))
            return (false, "학생코드가 존재하지 않습니다.");
        if (await _users.ExistsAsync(userId))
            return (false, "이미 존재하는 사용자입니다.");
        var existingByStudent = await _users.GetByStudentCodeAsync(studentCode);
        if (existingByStudent is not null)
            return (false, "해당 학생코드로 이미 가입된 계정이 있습니다.");

        var user = new User
        {
            Id = userId,
            Name = name,
            // ★ BCrypt 사용
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(rawPassword), 
            Role = role,
            StudentCode = studentCode
        };

        await _users.AddAsync(user);
        await _users.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool ok, string? token, string? error)> LoginAsync(string userId, string rawPassword)
    {
        var user = await _users.GetByIdAsync(userId);
        if (user is null) return (false, null, "아이디 또는 비밀번호가 올바르지 않습니다.");

        // ★ BCrypt 검증
        if (!BCrypt.Net.BCrypt.Verify(rawPassword, user.PasswordHash))
            return (false, null, "아이디 또는 비밀번호가 올바르지 않습니다.");

        var token = _jwt.Create(user);
        // ──────────────────────────────────────────────────────

        return (true, token, null);
    }
}
