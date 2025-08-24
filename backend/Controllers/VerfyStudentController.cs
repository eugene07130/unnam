using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UnnamHS_App_Backend.DTOs;
using UnnamHS_App_Backend.Services;

namespace UnnamHS_App_Backend.Controllers;

[ApiController]
[Route("api/verify-student")] // 서비스 역할에 맞춘 라우트
public class VerifyStudentController : ControllerBase
{
    private readonly IVerifyStudentService _verify;

    public VerifyStudentController(IVerifyStudentService verify)
    {
        _verify = verify;
    }

    /// <summary>
    /// 학생코드 검증 (존재/점유/사용가능 여부)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(VerifyStudentResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<VerifyStudentResponse>> Verify([FromBody] VerifyStudentRequest req)
    {
        var student = await _verify.GetByCodeAsync(req.StudentCode);
        var isUsable = await _verify.IsUsableAsync(req.StudentCode);

        var exists = student is not null;
        var isTaken = exists && !isUsable;

        var resp = new VerifyStudentResponse
        {
            StudentCode = req.StudentCode,
            Exists = exists,
            IsTaken = isTaken,
            IsUsable = isUsable,
            Student = student is null ? null : new StudentSummaryDto
            {
                StudentCode = student.StudentCode,
                Name = student.Name
            }
        };

        return Ok(resp);
    }

    /// <summary>
    /// 학생코드로 학생 기본 정보 조회(존재시 코드/이름 반환)
    /// </summary>
    [HttpGet("{studentCode}")]
    [ProducesResponseType(typeof(StudentSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentSummaryDto>> GetByCode(
        [FromRoute, Required, StringLength(16, MinimumLength = 1)] string studentCode)
    {
        var student = await _verify.GetByCodeAsync(studentCode);
        if (student is null) return NotFound();

        return Ok(new StudentSummaryDto
        {
            StudentCode = student.StudentCode,
            Name = student.Name
        });
    }

    /// <summary>
    /// 학생코드 사용 가능 여부만 빠르게 확인 (true/false)
    /// - true: students에 존재 && users.StudentCode로 미연결
    /// </summary>
    [HttpGet("{studentCode}/usable")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> IsUsable(
        [FromRoute, Required, StringLength(16, MinimumLength = 1)] string studentCode)
    {
        var usable = await _verify.IsUsableAsync(studentCode);
        return Ok(new { studentCode, isUsable = usable });
    }
}
