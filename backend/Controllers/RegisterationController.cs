using Microsoft.AspNetCore.Mvc;
using UnnamHS_App_Backend.DTOs;
using UnnamHS_App_Backend.Services;

namespace UnnamHS_App_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistrationController : ControllerBase
{
    private readonly IRegistrationService _regService;
    private readonly IStudentVerifyService  _codeService;

    public RegistrationController(IRegistrationService regService,
                                  IStudentVerifyService  codeService)
    {
        _regService  = regService;
        _codeService = codeService;
    }

    /// <summary>
    /// 학생코드 유효 검사
    /// </summary>
    [HttpPost("verify-student")]
    public async Task<IActionResult> VerifyStudent([FromBody] VerifyStudentDto dto)
    {
        var valid = await _codeService.IsValidAsync(dto.StudentCode);
        return Ok(new { valid });
    }

    /// <summary>
    /// 회원가입
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        var result = await _regService.RegisterAsync(dto);
        return result switch
        {
            RegisterResult.Success             => Ok("회원가입 성공"),
            RegisterResult.UsernameTaken       => Conflict("아이디 중복"),
            RegisterResult.CodeAlreadyUsed     => Conflict("이미 사용된 학생코드"),
            RegisterResult.InvalidCode         => BadRequest("유효하지 않은 학생코드"),
            _                                  => StatusCode(500)
        };
    }
}
