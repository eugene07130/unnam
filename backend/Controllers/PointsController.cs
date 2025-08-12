using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnnamHS_App_Backend.DTOs;
using UnnamHS_App_Backend.Repositories;

namespace UnnamHS_App_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]  // 로그인된 사용자만 접근
public class PointsController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public PointsController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// 내 포인트 조회
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPoints()
    {
        // ① 클레임에서 username 추출
        var username = User.FindFirstValue(ClaimTypes.Name);
        if (string.IsNullOrEmpty(username))
            return Unauthorized();

        // ② 사용자 조회
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
            return NotFound();

        // ③ 응답 DTO로 반환
        var response = new PointsResponseDto(user.Username, user.Points);
        return Ok(response);
    }

    /// <summary>
    /// 포인트 적립
    /// </summary>
    [HttpPost("add")]
    public async Task<IActionResult> AddPoints([FromBody] PointRequestDto dto)
    {
        // ① 클레임에서 username 추출
        var username = User.FindFirstValue(ClaimTypes.Name);
        if (string.IsNullOrEmpty(username))
            return Unauthorized();

        // ② 포인트 적립 시도
        var success = await _userRepository.AddPointsAsync(username, dto.Amount);
        if (!success)
            return NotFound("User not found");

        return Ok(new { message = "Points added", current = (await _userRepository.GetByUsernameAsync(username))?.Points });
    }

    /// <summary>
    /// 포인트 사용
    /// </summary>
    [HttpPost("use")]
    public async Task<IActionResult> UsePoints([FromBody] PointRequestDto dto)
    {
        // ① 클레임에서 username 추출
        var username = User.FindFirstValue(ClaimTypes.Name);
        if (string.IsNullOrEmpty(username))
            return Unauthorized();

        // ② 포인트 사용 시도
        var success = await _userRepository.UsePointsAsync(username, dto.Amount);
        if (!success)
            return BadRequest("Insufficient points or user not found");

        return Ok(new { message = "Points used", current = (await _userRepository.GetByUsernameAsync(username))?.Points });
    }
}

