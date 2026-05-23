using InventoryManagementSystem.Models;
using InventoryManagementSystem.ViewModels;

namespace InventoryManagementSystem.Services
{
    public interface IEmployeeService
    {
        Task<Employee?> GetEmployeeByIdAsync(int id, bool includeDeleted = false);
        Task<EmployeeDetailsViewModel?> GetEmployeeDetailsAsync(int id);
        Task<PagedResult<Employee>> GetPagedAsync(EmployeeFilterViewModel filter);
        Task<Employee?> GetEmployeeByUserIdAsync(int userId);
        Task AddEmployeeAsync(Employee employee, IEnumerable<int> categoryIds, int actingUserId);
        Task UpdateEmployeeAsync(Employee employee, IEnumerable<int> categoryIds, int actingUserId);
        Task<(bool Success, string? Error)> SoftDeleteAsync(int id, int actingUserId);
        Task SetActiveStatusAsync(int id, bool isActive, int actingUserId);
        Task<Employee> OnboardAsync(EmployeeOnboardViewModel model, int actingUserId);
        Task<IReadOnlyList<Employee>> GetManagersAsync(int? excludeEmployeeId = null);
        Task<(bool Success, string? Error)> ApplySelfRegistrationAsync(int userId, EmployeeSelfRegisterViewModel model);
        Task<IReadOnlyList<Employee>> GetPendingApprovalsAsync();
        Task ApproveEmployeeAsync(int employeeId, int actingUserId);
        Task RejectEmployeeAsync(int employeeId, int actingUserId, string? reason = null);
    }
}
