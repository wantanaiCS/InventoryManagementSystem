namespace InventoryManagementSystem.Models
{
    public class AuditLog
    {
        public int AuditId { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; } = null!; // "CREATE", "UPDATE", "DELETE"
        public string TableName { get; set; } = null!;
        public int RecordId { get; set; }
        public string OldValues { get; set; } = string.Empty; // JSON format
        public string NewValues { get; set; } = string.Empty; // JSON format
        public DateTime Timestamp { get; set; } = DateTime.Now;

        // Navigation property
        public User User { get; set; } = null!;
    }
}
