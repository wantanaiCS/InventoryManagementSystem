namespace InventoryManagementSystem.Models
{
    public class InventoryTransaction
    {
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public string TransactionType { get; set; } = null!; // "IN" or "OUT"
        public int Quantity { get; set; }
        public int CreatedBy { get; set; } // UserId
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Notes { get; set; } = string.Empty;

        // Navigation properties
        public Product Product { get; set; } = null!;
        public User CreatedByUser { get; set; } = null!;
    }
}
