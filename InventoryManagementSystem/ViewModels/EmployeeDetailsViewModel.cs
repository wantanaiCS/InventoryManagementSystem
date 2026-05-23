using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.ViewModels
{
    public class EmployeeDetailsViewModel
    {
        public Employee Employee { get; set; } = null!;
        public IReadOnlyList<InventoryTransaction> RecentTransactions { get; set; } = Array.Empty<InventoryTransaction>();
        public IReadOnlyList<AuditLog> AuditTrail { get; set; } = Array.Empty<AuditLog>();
        public int TransactionsToday { get; set; }
        public int TransactionsThisWeek { get; set; }
        public int TotalInQuantity { get; set; }
        public int TotalOutQuantity { get; set; }
        public IReadOnlyList<ActivityItem> ActivityTimeline { get; set; } = Array.Empty<ActivityItem>();
    }

    public class ActivityItem
    {
        public DateTime Timestamp { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = "fa-circle";
        public string BadgeClass { get; set; } = "bg-secondary";
    }
}
