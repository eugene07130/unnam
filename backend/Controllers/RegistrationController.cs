using Microsoft.AspNetCore.Mvc;
using UnnamHS_App_Backend.DTOs;
using UnnamHS_App_Backend.Services;

namespace UnnamHS_App_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistrationController : ControllerBase
{
    private readonly IRegistrationService _regService;
    private readonly IStudentVerifyService       _verify; // 이름/시그니처 통일

    public RegistrationController(IRegistrationService regService,
                                  IStudentVerifyService       verify)
    {
        _regService = regService;
        _verify     = verify;
    }

    /// <summary>학생코드 사용 가능 여부(존재 또는 미사용)</summary>
    [HttpPost("check")]
    public async Task<IActionResult> Check([FromBody] VerifyStudentDto dto)
    {
        var usable = await _verify.IsUsableAsync(dto.StudentCode);
        return Ok(new { usable });
    }

    /// <summary>회원가입</summary>
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        var result = await _regService.RegisterAsync(dto);
        return result switch
        {
            RegisterResult.Success         => Ok("회원가입 성공"),
            RegisterResult.UsernameTaken   => Conflict("아이디 중복"),
            RegisterResult.CodeAlreadyUsed => Conflict("이미 사용된 학생코드"),
            RegisterResult.InvalidCode     => BadRequest("유효하지 않은 학생코드"),
            _                              => StatusCode(500)
        };
    }
}
