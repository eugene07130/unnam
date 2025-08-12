using UnnamHS_App_Backend.Models;

namespace UnnamHS_App_Backend.Repositories;

public interface IPointHistoryRepository
{
    Task AddAsync(PointHistory entry);                       // {StudentCode, Source, Points, CreatedAt(UTC)}
    Task<int> GetTotalAsync(string studentCode);             // SUM(points)
    Task<IReadOnlyList<PointHistory>> GetRecentAsync(string studentCode, int take = 50);
}
