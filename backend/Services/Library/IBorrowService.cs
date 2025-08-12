namespace UnnamHS_App_Backend.Services;
using UnnamHS_App_Backend.Models;

public interface IBorrowService
{
    Task<long> BorrowAsync(string userId, string bookCode, DateTime borrowedAtUtc);
    Task<bool> ReturnAsync(long recordId);
    Task<bool> ExtendAsync(long recordId); // 1주 연장(최대 2회)
    Task<IReadOnlyList<BorrowRecord>> GetOpenByUserAsync(string userId);
}
