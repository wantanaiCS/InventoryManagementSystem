namespace InventoryManagementSystem.ViewModels
{
    public class EmployeeFilterViewModel
    {
        public string SearchTerm { get; set; } = string.Empty;
        public int? DepartmentId { get; set; }
        public string? Position { get; set; }
        public bool? IsActive { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
