namespace InventoryManagementSystem.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Position { get; set; } = null!;
        public DateTime HireDate { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // Navigation property
        public User User { get; set; } = null!;
    }
}
