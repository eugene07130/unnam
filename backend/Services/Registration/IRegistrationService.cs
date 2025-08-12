using System.Threading.Tasks;
using UnnamHS_App_Backend.DTOs;

namespace UnnamHS_App_Backend.Services;

public interface IRegistrationService
{
    /// <summary>
    /// 학생코드 기반 회원가입 수행.
    /// </summary>
    Task<RegisterResult> RegisterAsync(RegisterUserDto dto);
}
