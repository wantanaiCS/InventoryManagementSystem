using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.ViewModels
{
    public class InventoryTransactionFormViewModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public string Notes { get; set; } = string.Empty;
        public string Shift { get; set; } = "Morning";
    }
}
