using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace InventoryManagementSystem.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public int CurrentStock { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Furniture-specific
        public string Material { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public decimal? WidthCm { get; set; }
        public decimal? DepthCm { get; set; }
        public decimal? HeightCm { get; set; }
        public string Unit { get; set; } = "ชิ้น";
        public string WarehouseLocation { get; set; } = string.Empty;

        [ValidateNever]
        public Category? Category { get; set; }
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();

        public string DimensionDisplay =>
            WidthCm.HasValue && DepthCm.HasValue && HeightCm.HasValue
                ? $"{WidthCm}×{DepthCm}×{HeightCm} cm"
                : "—";
    }
}
