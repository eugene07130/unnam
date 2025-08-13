using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnnamHS_App_Backend.Services;
using UnnamHS_App_Backend.DTOs;

namespace UnnamHS_App_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    // POST: /api/auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var (ok, token, error) = await _auth.LoginAsync(dto.UserId, dto.Password);
        if (ok && !string.IsNullOrEmpty(token))
            return Ok(new { token });

        return Unauthorized(new { error = error ?? "invalid_credentials" });
    }
}