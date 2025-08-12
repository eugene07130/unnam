using Microsoft.EntityFrameworkCore;
using UnnamHS_App_Backend.Models;
using UnnamHS_App_Backend.Data;    

namespace UnnamHS_App_Backend.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;
    public OrderService(AppDbContext db) => _db = db;

    public async Task<long> CreateAsync(string studentCode, bool toGo, string? request,
        List<(int menuId, int qty, int? unitPrice, List<int> optionIds)> lines)
    {
        if (!await _db.Students.AnyAsync(s => s.StudentCode == studentCode))
            throw new InvalidOperationException("학생코드 없음");
        if (lines.Count == 0) throw new InvalidOperationException("주문 라인이 비었습니다.");

        var order = new Order
        {
            StudentCode = studentCode,
            OrderedAt = DateTime.UtcNow,
            Status = "pending",
            ToGo = toGo,
            Request = request,
            TotalPrice = 0
        };
        _db.Orders.Add(order);
        await _db.SaveChangesAsync(); // OrderId 확보

        var lineNo = 1;
        foreach (var (menuId, qty, unitPrice, optionIds) in lines)
        {
            var menu = await _db.MenuItems.FindAsync(menuId) ?? throw new InvalidOperationException($"메뉴 {menuId} 없음");
            var price = unitPrice ?? menu.BasePrice;

            var line = new OrderLine
            {
                OrderId = order.OrderId,
                LineNo = lineNo++,
                MenuId = menuId,
                Qty = qty,
                UnitPrice = price,
                LinePrice = price * qty,
            };
            _db.OrderLines.Add(line);

            if (optionIds is { Count: > 0 })
            {
                foreach (var optId in optionIds)
                {
                    var opt = await _db.OptionItems.FindAsync(optId) ?? throw new InvalidOperationException($"옵션 {optId} 없음");
                    _db.OrderLineOptions.Add(new OrderLineOption
                    {
                        OrderId = order.OrderId,
                        LineNo = line.LineNo,
                        OptionId = optId
                    });
                    line.LinePrice += opt.PriceDelta * qty; // 옵션가 반영
                }
            }
        }
        await _db.SaveChangesAsync();

        await RecalcOrderTotalAsync(order.OrderId);
        return order.OrderId;
    }

    public async Task<bool> ChangeStatusAsync(long orderId, string newStatus)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        if (order is null) return false;

        // 간단한 상태 전이 검증
        var ok = (order.Status, newStatus) switch
        {
            ("pending", "paid") => true,
            ("paid", "ready") => true,
            ("ready", "done") => true,
            _ => false
        };
        if (!ok) return false;

        order.Status = newStatus;
        await _db.SaveChangesAsync();
        return true;
    }

        public async Task<Order?> GetAsync(long orderId, bool includeLines)
    {
        IQueryable<Order> q = _db.Orders.AsQueryable();
        if (includeLines)
        {
            q = q
                .Include(o => o.Lines)
                    .ThenInclude(l => l.Options)       // ICollection<OrderLineOption>
                .Include(o => o.Lines)
                    .ThenInclude(l => l.MenuItem);     // 필요하면
        }
        return await q.FirstOrDefaultAsync(o => o.OrderId == orderId);
    }


    private async Task RecalcOrderTotalAsync(long orderId)
    {
        var sum = await _db.OrderLines
            .Where(l => l.OrderId == orderId)
            .SumAsync(l => (int?)l.LinePrice ?? 0);
        var order = await _db.Orders.FirstAsync(o => o.OrderId == orderId);
        order.TotalPrice = sum;
        await _db.SaveChangesAsync();
    }
}
