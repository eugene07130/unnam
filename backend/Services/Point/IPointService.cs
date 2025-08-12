namespace UnnamHS_App_Backend.Services;

public interface IPointsService
{
    Task<int> GetTotalAsync(string studentCode);
    Task AddAsync(string studentCode, int points, string source);
    Task<bool> TryUseAsync(string studentCode, int points, string source); // 합계 체크 후 차감 기록
}

