using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using UnnamHS_App_Backend.Data;
using UnnamHS_App_Backend.DTOs;
using UnnamHS_App_Backend.Models;

namespace UnnamHS_App_Backend.Services;

/// <summary>
/// 회원가입(Registration) 구현체
/// - DB 제약 준수: students 존재 확인, users.Id 중복 불가, users.StudentCode UNIQUE
/// </summary>
public sealed class RegistrationService : IRegistrationService
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher? _hasher;

    public RegistrationService(AppDbContext db, IPasswordHasher? hasher = null)
    {
        _db = db;
        _hasher = hasher;
    }

    public async Task<bool> IsUserIdAvailableAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return false;
        userId = userId.Trim();
        return !await _db.Users.AnyAsync(u => u.Id == userId);
    }

    public async Task<RegisterResult> RegisterAsync(RegisterUserDto dto)
    {
        // 1) 기본 검증 (길이/공백)
        var userId = (dto.UserId ?? "").Trim();
        var name = (dto.Name ?? "").Trim();
        var studentCode = (dto.StudentCode ?? "").Trim();
        var password = dto.Password ?? "";

        if (userId.Length is < 3 or > 64)
            throw new ArgumentException("UserId must be 3-64 characters.");
        if (name.Length is < 1 or > 64)
            throw new ArgumentException("Name must be 1-64 characters.");
        if (studentCode.Length is < 1 or > 16)
            throw new ArgumentException("StudentCode must be 1-16 characters.");
        if (password.Length is < 8 or > 128)
            throw new ArgumentException("Password must be 8-128 characters.");

        // 2) 학생코드 존재 확인 (students에 등록되어 있어야 함)
        var student = await _db.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.StudentCode == studentCode);
        if (student is null)
            throw new ArgumentException("Invalid StudentCode: not found in students.");

        // 3) userId 중복 사전 검사
        if (await _db.Users.AnyAsync(u => u.Id == userId))
            throw new InvalidOperationException("UserId already exists.");

        // 4) 해당 StudentCode가 이미 다른 사용자에 연결되어 있는지 검사 (UNIQUE(users.StudentCode))
        if (await _db.Users.AnyAsync(u => u.StudentCode == studentCode))
            throw new InvalidOperationException("StudentCode is already linked to another user.");

        // 5) 비밀번호 해시
        var passwordHash = _hasher is not null
            ? _hasher.Hash(password)
            : HashPasswordSha256(password);

        // 6) User 엔티티 생성 (Role='student' 고정)
        var entity = new User
        {
            Id = userId,
            PasswordHash = passwordHash,
            Name = name,
            Role = "student",
            StudentCode = studentCode
        };

        _db.Users.Add(entity);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // 이중 보증: DB 레벨 UNIQUE 충돌 등
            // - users.Id UNIQUE(기본키)
            // - users.StudentCode UNIQUE 인덱스(컨텍스트에 선언됨)
            throw new InvalidOperationException("Registration failed due to constraint violation.", ex);
        }

        return new RegisterResult
        {
            UserId = entity.Id,
            Name = entity.Name,
            StudentCode = entity.StudentCode,
            Role = entity.Role
        };
    }

    // 간단한 SHA256+Salt(16바이트) 해시. 저장 형식: "{saltBase64}:{hashBase64}"
    private static string HashPasswordSha256(string password)
    {
        Span<byte> salt = stackalloc byte[16];
        RandomNumberGenerator.Fill(salt);

        using var sha = SHA256.Create();
        var pwBytes = Encoding.UTF8.GetBytes(password);

        byte[] combined = new byte[salt.Length + pwBytes.Length];
        salt.CopyTo(combined);
        Buffer.BlockCopy(pwBytes, 0, combined, salt.Length, pwBytes.Length);

        var hash = sha.ComputeHash(combined);
        return $"{Convert.ToBase64String(salt.ToArray())}:{Convert.ToBase64String(hash)}";
    }
}

/// <summary>
/// 선택 DI용 비밀번호 해셔(프로젝트에 이미 있다면 그걸 사용)
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
}
