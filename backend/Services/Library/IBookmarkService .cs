namespace UnnamHS_App_Backend.Services;
using UnnamHS_App_Backend.Models;

public interface IBookmarkService
{
    Task<bool> AddAsync(string userId, string bookCode);
    Task<bool> RemoveAsync(string userId, string bookCode);
    Task<IReadOnlyList<Bookmark>> ListAsync(string userId);
}
