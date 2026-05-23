using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace InventoryManagementSystem.Models
{
    public class InventoryTransaction
    {
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public string TransactionType { get; set; } = null!;
        public int Quantity { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Notes { get; set; } = string.Empty;
        public string Shift { get; set; } = "Morning";

        [ValidateNever]
        public Product? Product { get; set; }

        [ValidateNever]
        public User? CreatedByUser { get; set; }
    }
}
