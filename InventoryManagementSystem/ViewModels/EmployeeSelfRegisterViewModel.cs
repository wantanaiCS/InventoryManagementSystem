using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.ViewModels
{
    public class EmployeeSelfRegisterViewModel
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Position { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; } = DateTime.Today;

        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int? DepartmentId { get; set; }
        public string Shift { get; set; } = "Morning";
    }
}
