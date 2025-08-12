using Microsoft.AspNetCore.Mvc;
using UnnamHS_App_Backend.DTOs;
using UnnamHS_App_Backend.Services;   // IAuthService

namespace UnnamHS_App_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// 아이디·비밀번호로 로그인 → JWT 발급.
    /// 실패 시 401 Unauthorized.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var response = await _authService.AuthenticateAsync(request);
        return response is null
            ? Unauthorized("아이디 또는 비밀번호가 올바르지 않습니다.")
            : Ok(response);
    }
}
