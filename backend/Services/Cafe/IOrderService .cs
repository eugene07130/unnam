namespace UnnamHS_App_Backend.Services;
using UnnamHS_App_Backend.Models;
public interface IOrderService
{
    Task<long> CreateAsync(string studentCode, bool toGo, string? request, List<(int menuId, int qty, int? unitPrice, List<int> optionIds)> lines);
    Task<bool> ChangeStatusAsync(long orderId, string newStatus); // "pending"→"paid"→"ready"→"done"
    Task<Order?> GetAsync(long orderId, bool includeLines = true);
}
