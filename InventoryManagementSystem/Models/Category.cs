namespace InventoryManagementSystem.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        /// <summary>Furniture, General, etc.</summary>
        public string ProductLine { get; set; } = "General";

        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<EmployeeCategoryAssignment> EmployeeAssignments { get; set; } = new List<EmployeeCategoryAssignment>();
    }
}
