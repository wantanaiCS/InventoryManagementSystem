using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.ViewModels
{
    public class DashboardViewModel
    {
        public bool IsAdmin { get; set; }
        public int TotalProducts { get; set; }
        public int TotalStock { get; set; }
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int TransactionsToday { get; set; }
        public int MyTransactionsToday { get; set; }
        public IReadOnlyList<Product> LowStockProducts { get; set; } = Array.Empty<Product>();
        public IReadOnlyList<InventoryTransaction> RecentTransactions { get; set; } = Array.Empty<InventoryTransaction>();
        public IReadOnlyList<AppNotification> Notifications { get; set; } = Array.Empty<AppNotification>();
        public EmployeePerformanceSnapshot? MyPerformance { get; set; }
    }

    public class EmployeePerformanceSnapshot
    {
        public int TransactionsThisWeek { get; set; }
        public int TotalIn { get; set; }
        public int TotalOut { get; set; }
        public string TopProductName { get; set; } = "—";
    }
}
