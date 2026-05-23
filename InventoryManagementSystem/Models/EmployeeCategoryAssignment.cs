using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace InventoryManagementSystem.Models
{
    public class EmployeeCategoryAssignment
    {
        public int EmployeeId { get; set; }
        public int CategoryId { get; set; }

        [ValidateNever]
        public Employee? Employee { get; set; }

        [ValidateNever]
        public Category? Category { get; set; }
    }
}
