using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnnamHS_App_Backend.DTOs;
using UnnamHS_App_Backend.Services;

namespace UnnamHS_App_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistrationController : ControllerBase
{
    private readonly IRegistrationService _regService;
    private readonly IStudentVerifyService _verify;

    public RegistrationController(
        IRegistrationService regService,
        IStudentVerifyService verify)
    {
        _regService = regService;
        _verify = verify;
    }

    /// <summary>
    /// 학생코드 사용 가능 여부(Students에 존재 및 Users에 미연결)
    /// </summary>
    [HttpPost("check")]
    [AllowAnonymous]
    public async Task<IActionResult> Check([FromBody] VerifyStudentDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var usable = await _verify.IsUsableAsync(dto.StudentCode);
        return Ok(new { usable });
    }

    /// <summary>
    /// 회원가입 (User.Id, Password, Name, StudentCode, Role)
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var result = await _regService.RegisterAsync(dto);
        return result switch
        {
            RegisterResult.Success         => Ok(new { message = "회원가입 성공" }),
            RegisterResult.UsernameTaken   => Conflict(new { error = "아이디 중복" }),
            RegisterResult.CodeAlreadyUsed => Conflict(new { error = "이미 사용된 학생코드" }),
            RegisterResult.InvalidCode     => BadRequest(new { error = "유효하지 않은 학생코드" }),
            _                              => StatusCode(500, new { error = "registration_failed" })
        };
    }
}
