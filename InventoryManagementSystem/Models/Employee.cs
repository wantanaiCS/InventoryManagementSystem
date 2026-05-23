using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace InventoryManagementSystem.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public int UserId { get; set; }
        public int? DepartmentId { get; set; }
        public int? ReportsToEmployeeId { get; set; }
        public string FullName { get; set; } = null!;
        public string Position { get; set; } = null!;
        public DateTime HireDate { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Shift { get; set; } = "Morning";
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }

        [ValidateNever]
        public User? User { get; set; }

        [ValidateNever]
        public Department? Department { get; set; }

        [ValidateNever]
        public Employee? ReportsTo { get; set; }

        [ValidateNever]
        public ICollection<Employee> DirectReports { get; set; } = new List<Employee>();

        [ValidateNever]
        public ICollection<EmployeeCategoryAssignment> CategoryAssignments { get; set; } = new List<EmployeeCategoryAssignment>();
    }
}
