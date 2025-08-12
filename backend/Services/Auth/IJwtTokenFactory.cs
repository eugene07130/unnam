using UnnamHS_App_Backend.Models;

namespace UnnamHS_App_Backend.Services;

public interface IJwtTokenFactory
{
    /// <summary>
    /// User 엔티티로부터 JWT를 생성한다.
    /// lifetime이 null이면 기본(예: 2h)을 사용한다.
    /// </summary>
    string Create(User user, TimeSpan? lifetime = null);
}
