namespace InventoryManagementSystem.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = null!; // UNIQUE
        public string ProductName { get; set; } = null!;
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public int CurrentStock { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public Category Category { get; set; } = null!;
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}
