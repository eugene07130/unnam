using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UnnamHS_App_Backend.DTOs;
using UnnamHS_App_Backend.Services;

namespace UnnamHS_App_Backend.Controllers;

[ApiController]
[Route("api/registration")]
public class RegistrationController : ControllerBase
{
    private readonly IRegistrationService _svc;

    public RegistrationController(IRegistrationService svc)
    {
        _svc = svc;
    }

    /// <summary>로그인 ID 가용성 확인</summary>
    [HttpGet("user-id/available")]
    [ProducesResponseType(typeof(UserIdAvailabilityDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserIdAvailabilityDto>> IsUserIdAvailable(
        [FromQuery, Required, StringLength(64, MinimumLength = 1)] string userId)
    {
        var ok = await _svc.IsUserIdAvailableAsync(userId);
        return Ok(new UserIdAvailabilityDto { UserId = userId, IsAvailable = ok });
    }

    /// <summary>회원가입(학생 전용 가정). 서비스 인터페이스에 맞춰 위임</summary>
    [HttpPost]
    [ProducesResponseType(typeof(RegisterResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RegisterResult>> Register([FromBody] RegisterUserDto dto)
    {
        try
        {
            var result = await _svc.RegisterAsync(dto);
            return Created($"/api/users/{result.UserId}", result);
        }
        catch (ArgumentException ex)
        {
            // 형식/검증 오류 등
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // 비즈니스 충돌: userId 중복, studentCode 이미 연동 등
            return Conflict(new { message = ex.Message });
        }
    }
}
