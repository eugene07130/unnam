using UnnamHS_App_Backend.DTOs;

namespace UnnamHS_App_Backend.Services;

public interface IRegistrationService
{
    /// <summary>학생코드 기반 회원가입 수행.</summary>
    Task<RegisterResult> RegisterAsync(RegisterUserDto dto);

    /// <summary>로그인 ID 가용성(중복) 확인.</summary>
    Task<bool> IsUserIdAvailableAsync(string userId);
}
