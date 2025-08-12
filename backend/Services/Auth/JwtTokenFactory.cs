using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UnnamHS_App_Backend.Models;

namespace UnnamHS_App_Backend.Services;

public class JwtTokenFactory : IJwtTokenFactory
{
    private readonly IConfiguration _config;

    public JwtTokenFactory(IConfiguration config) => _config = config;

    public string Create(User user, TimeSpan? lifetime = null)
    {
        var key    = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
        var issuer = _config["Jwt:Issuer"];
        var aud    = _config["Jwt:Audience"];

        // HS256은 최소 32바이트(256비트) 이상 권장
        if (Encoding.UTF8.GetBytes(key).Length < 32)
            throw new InvalidOperationException("Jwt:Key must be at least 32 bytes for HS256.");

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        // Program.cs 에서 RoleClaimType = "role" 이므로 여기서도 "role" 사용
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),                    // sub
            new(ClaimTypes.Name, user.Name ?? user.Id),
            new("role", user.Role),
        };

        if (!string.IsNullOrWhiteSpace(user.StudentCode))
            claims.Add(new("studentCode", user.StudentCode));

        var now = DateTime.UtcNow;
        var ttl = lifetime ?? GetDefaultLifetimeFromConfig() ?? TimeSpan.FromHours(2);

        var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: aud,
            claims: claims,
            notBefore: now,
            expires: now.Add(ttl),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    private TimeSpan? GetDefaultLifetimeFromConfig()
    {
        // appsettings.json 에 "Jwt:LifetimeMinutes": 120 식으로 넣으면 사용
        var minutes = _config["Jwt:LifetimeMinutes"];
        return int.TryParse(minutes, out var m) ? TimeSpan.FromMinutes(m) : null;
    }
}
