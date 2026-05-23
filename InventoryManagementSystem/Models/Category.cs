namespace InventoryManagementSystem.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string Description { get; set; } = string.Empty;

        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<EmployeeCategoryAssignment> EmployeeAssignments { get; set; } = new List<EmployeeCategoryAssignment>();
    }
}
