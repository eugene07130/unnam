using Microsoft.EntityFrameworkCore;
using UnnamHS_App_Backend.Data;
using UnnamHS_App_Backend.DTOs;
using UnnamHS_App_Backend.Models;
using UnnamHS_App_Backend.Repositories;
using Microsoft.AspNetCore.Identity;

namespace UnnamHS_App_Backend.Services;

public static class RoleConstants
{
    public const string Student   = "student";
    public const string Admin     = "admin";
    public const string Kiosk     = "kiosk";
    public const string CafeKiosk = "cafe_kiosk";
}

public class RegistrationService : IRegistrationService
{
    private readonly AppDbContext _db;
    private readonly IUserRepository _users;
    private readonly IStudentVerifyService _verify;
    private readonly IPasswordHasher<User> _hasher;

    public RegistrationService(
        AppDbContext db,
        IUserRepository users,
        IStudentVerifyService verify,
        IPasswordHasher<User> hasher)
    {
        _db = db; _users = users; _verify = verify; _hasher = hasher;
    }

    public async Task<RegisterResult> RegisterAsync(RegisterUserDto dto)
    {
        var userId = (dto.UserId ?? string.Empty).Trim();
        var studentCode = (dto.StudentCode ?? string.Empty).Trim();
        var password = dto.Password ?? string.Empty;

        // 정책: UserId는 소문자로 정규화 (선택)
        userId = userId.ToLowerInvariant();

        if (userId.Length < 3 || password.Length < 8 || studentCode.Length < 5)
            return RegisterResult.UsernameTaken; // 컨트롤러 단 검증 권장

        // 학생코드 사용 가능?
        if (!await _verify.IsUsableAsync(studentCode))
            return RegisterResult.InvalidCode;

        // ID 중복?
        if (await _users.ExistsAsync(userId))
            return RegisterResult.UsernameTaken;

        var student = await _db.Students.AsNoTracking()
            .FirstOrDefaultAsync(s => s.StudentCode == studentCode);

        if (student is null)
            return RegisterResult.InvalidCode;

        var user = new User
        {
            Id = userId,
            Role = RoleConstants.Student, // 문자열 하드코딩 회피
            StudentCode = studentCode,
            Name = student.Name
        };

        user.PasswordHash = _hasher.HashPassword(user, password);

        // (선택) 짧은 트랜잭션으로 경쟁 상태 창 줄이기
        // await using var tx = await _db.Database.BeginTransactionAsync();
        await _users.AddAsync(user);
        try
        {
            await _users.SaveChangesAsync();
            // await tx.CommitAsync();
        }
        catch (DbUpdateException)
        {
            // UNIQUE(StudentCode) or UNIQUE(Id) 충돌
            // 충돌 키를 파싱하면 더 정확히 분기 가능하나, 간단하게 처리
            // Id 중복: UsernameTaken, Code 중복: CodeAlreadyUsed 로 분리하고 싶다면 예외 메시지 파싱 필요
            return RegisterResult.CodeAlreadyUsed;
        }

        return RegisterResult.Success;
    }
}
