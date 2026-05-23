using InventoryManagementSystem.Data;
using InventoryManagementSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public DashboardService(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<DashboardViewModel> GetDashboardAsync(int userId, bool isAdmin)
        {
            var today = DateTime.Today;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);

            var vm = new DashboardViewModel
            {
                IsAdmin = isAdmin,
                TotalProducts = await _context.Products.CountAsync(),
                TotalStock = await _context.Products.SumAsync(p => p.CurrentStock),
                TotalEmployees = await _context.Employees.CountAsync(),
                ActiveEmployees = await _context.Employees.CountAsync(e => e.IsActive),
                TransactionsToday = await _context.InventoryTransactions.CountAsync(t => t.CreatedDate.Date == today),
                MyTransactionsToday = await _context.InventoryTransactions.CountAsync(t => t.CreatedBy == userId && t.CreatedDate.Date == today),
                LowStockProducts = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.CurrentStock <= 5)
                    .OrderBy(p => p.CurrentStock)
                    .Take(10)
                    .AsNoTracking()
                    .ToListAsync(),
                RecentTransactions = await _context.InventoryTransactions
                    .Include(t => t.Product)
                    .Include(t => t.CreatedByUser)
                    .OrderByDescending(t => t.CreatedDate)
                    .Take(isAdmin ? 15 : 8)
                    .AsNoTracking()
                    .ToListAsync(),
                Notifications = await _notificationService.GetUnreadAsync(userId, 8)
            };

            var myTx = await _context.InventoryTransactions
                .Include(t => t.Product)
                .Where(t => t.CreatedBy == userId && t.CreatedDate >= weekStart)
                .AsNoTracking()
                .ToListAsync();

            var topProduct = myTx
                .GroupBy(t => t.Product?.ProductName ?? "Unknown")
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "—";

            vm.MyPerformance = new EmployeePerformanceSnapshot
            {
                TransactionsThisWeek = myTx.Count,
                TotalIn = myTx.Where(t => t.TransactionType == "IN").Sum(t => t.Quantity),
                TotalOut = myTx.Where(t => t.TransactionType == "OUT").Sum(t => t.Quantity),
                TopProductName = topProduct
            };

            return vm;
        }
    }
}
