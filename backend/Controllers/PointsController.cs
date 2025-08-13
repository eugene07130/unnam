using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnnamHS_App_Backend.DTOs;
using UnnamHS_App_Backend.Services;

namespace UnnamHS_App_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PointsController : ControllerBase
{
    private readonly IPointsService _points;

    public PointsController(IPointsService points)
    {
        _points = points;
    }

    // JWT에서 studentCode 꺼내기
    private string? GetStudentCode()
        => User.FindFirst("studentCode")?.Value;

    /// <summary>내 포인트 합계</summary>
    [HttpGet]
    public async Task<IActionResult> GetMyPoints()
    {
        var code = GetStudentCode();
        if (string.IsNullOrWhiteSpace(code))
            return Unauthorized("studentCode 없음");

        var total = await _points.GetTotalAsync(code);
        return Ok(new { studentCode = code, total });
    }

    /// <summary>포인트 적립</summary>
    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] PointRequestDto dto)
    {
        var code = GetStudentCode();
        if (string.IsNullOrWhiteSpace(code))
            return Unauthorized("studentCode 없음");

        await _points.AddAsync(code, dto.Amount, dto.Source ?? "manual");
        var total = await _points.GetTotalAsync(code);
        return Ok(new { message = "added", total });
    }

    /// <summary>포인트 사용</summary>
    [HttpPost("use")]
    public async Task<IActionResult> Use([FromBody] PointRequestDto dto)
    {
        var code = GetStudentCode();
        if (string.IsNullOrWhiteSpace(code))
            return Unauthorized("studentCode 없음");

        var ok = await _points.TryUseAsync(code, dto.Amount, dto.Source ?? "use");
        if (!ok) return BadRequest("포인트 부족");

        var total = await _points.GetTotalAsync(code);
        return Ok(new { message = "used", total });
    }
}
