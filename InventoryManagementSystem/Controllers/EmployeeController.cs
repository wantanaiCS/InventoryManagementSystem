using InventoryManagementSystem.Attributes;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Helpers;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IEmployeeService employeeService, ApplicationDbContext context, ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(EmployeeFilterViewModel filter)
        {
            var result = await _employeeService.GetPagedAsync(filter);
            ViewBag.Filter = filter;
            ViewBag.Departments = await _context.Departments.OrderBy(d => d.DepartmentName).ToListAsync();
            ViewBag.IsAdmin = HttpContext.Session.IsAdmin();
            if (HttpContext.Session.IsAdmin())
                ViewBag.PendingCount = (await _employeeService.GetPendingApprovalsAsync()).Count;
            return View(result);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var vm = await _employeeService.GetEmployeeDetailsAsync(id.Value);
            if (vm == null) return NotFound();
            ViewBag.IsAdmin = HttpContext.Session.IsAdmin();
            return View(vm);
        }

        [Authorize("Admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateFormDataAsync();
            return View(new Employee { HireDate = DateTime.Today, IsActive = true, Shift = "Morning" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Admin")]
        public async Task<IActionResult> Create(
            [Bind("UserId,FullName,Position,HireDate,PhoneNumber,Address,DepartmentId,ReportsToEmployeeId,Shift,IsActive")] Employee employee,
            List<int> categoryIds)
        {
            if (employee.UserId <= 0)
                ModelState.AddModelError(nameof(employee.UserId), "Please select a user account.");

            if (await _employeeService.GetEmployeeByUserIdAsync(employee.UserId) != null)
                ModelState.AddModelError(nameof(employee.UserId), "This user already has an employee record");

            if (ModelState.IsValid)
            {
                try
                {
                    var actingUserId = HttpContext.Session.GetCurrentUserId() ?? 0;
                    await _employeeService.AddEmployeeAsync(employee, categoryIds ?? new List<int>(), actingUserId);
                    TempData["Success"] = "Employee created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Create employee failed");
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the employee.");
                }
            }

            await PopulateFormDataAsync(employee.UserId);
            return View(employee);
        }

        [Authorize("Admin")]
        public async Task<IActionResult> Onboard()
        {
            await PopulateOnboardFormDataAsync();
            return View(new EmployeeOnboardViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Admin")]
        public async Task<IActionResult> Onboard(EmployeeOnboardViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var actingUserId = HttpContext.Session.GetCurrentUserId() ?? 0;
                    var employee = await _employeeService.OnboardAsync(model, actingUserId);
                    TempData["Success"] = $"Onboarding complete for {employee.FullName}.";
                    return RedirectToAction(nameof(Details), new { id = employee.EmployeeId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Onboard failed");
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            await PopulateOnboardFormDataAsync();
            return View(model);
        }

        [Authorize("Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var employee = await _employeeService.GetEmployeeByIdAsync(id.Value);
            if (employee == null) return NotFound();
            await PopulateFormDataAsync(employee.UserId, id);
            ViewBag.SelectedCategories = employee.CategoryAssignments.Select(c => c.CategoryId).ToList();
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Admin")]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("EmployeeId,UserId,FullName,Position,HireDate,PhoneNumber,Address,DepartmentId,ReportsToEmployeeId,Shift,IsActive")] Employee employee,
            List<int> categoryIds)
        {
            if (id != employee.EmployeeId) return NotFound();

            if (ModelState.IsValid)
            {
                var actingUserId = HttpContext.Session.GetCurrentUserId() ?? 0;
                await _employeeService.UpdateEmployeeAsync(employee, categoryIds ?? new List<int>(), actingUserId);
                TempData["Success"] = "Employee updated.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await PopulateFormDataAsync(employee.UserId, id);
            ViewBag.SelectedCategories = categoryIds ?? new List<int>();
            return View(employee);
        }

        [Authorize("Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var employee = await _employeeService.GetEmployeeByIdAsync(id.Value);
            if (employee == null) return NotFound();
            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize("Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actingUserId = HttpContext.Session.GetCurrentUserId() ?? 0;
            var (success, error) = await _employeeService.SoftDeleteAsync(id, actingUserId);
            if (!success)
                TempData["Error"] = error;
            else
                TempData["Success"] = "Employee deactivated (soft delete).";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Admin")]
        public async Task<IActionResult> ToggleActive(int id, bool isActive)
        {
            var actingUserId = HttpContext.Session.GetCurrentUserId() ?? 0;
            await _employeeService.SetActiveStatusAsync(id, isActive, actingUserId);
            TempData["Success"] = isActive ? "Employee activated." : "Employee deactivated.";
            return RedirectToAction(nameof(Details), new { id });
        }

        /// <summary>พนักงานสมัครเอง (Auto) — รอ Admin อนุมัติ</summary>
        [Authorize]
        public async Task<IActionResult> ApplyProfile()
        {
            var userId = HttpContext.Session.GetCurrentUserId();
            if (!userId.HasValue) return RedirectToAction("Login", "Auth");

            var existing = await _employeeService.GetEmployeeByUserIdAsync(userId.Value);
            if (existing != null)
            {
                if (existing.ApprovalStatus == "Pending")
                    TempData["Info"] = "Your registration is pending admin approval.";
                return RedirectToAction(nameof(Details), new { id = existing.EmployeeId });
            }

            ViewBag.Departments = await _context.Departments.OrderBy(d => d.DepartmentName).ToListAsync();
            ViewBag.Shifts = new[] { "Morning", "Afternoon", "Night" };
            return View(new EmployeeSelfRegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ApplyProfile(EmployeeSelfRegisterViewModel model)
        {
            var userId = HttpContext.Session.GetCurrentUserId();
            if (!userId.HasValue) return RedirectToAction("Login", "Auth");

            if (ModelState.IsValid)
            {
                var (success, error) = await _employeeService.ApplySelfRegistrationAsync(userId.Value, model);
                if (success)
                {
                    TempData["Success"] = "Application submitted. An administrator will review your request.";
                    return RedirectToAction("Index", "Dashboard");
                }
                ModelState.AddModelError(string.Empty, error ?? "Could not submit application.");
            }

            ViewBag.Departments = await _context.Departments.OrderBy(d => d.DepartmentName).ToListAsync();
            ViewBag.Shifts = new[] { "Morning", "Afternoon", "Night" };
            return View(model);
        }

        [Authorize("Admin")]
        public async Task<IActionResult> PendingApprovals()
        {
            var list = await _employeeService.GetPendingApprovalsAsync();
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var actingUserId = HttpContext.Session.GetCurrentUserId() ?? 0;
            await _employeeService.ApproveEmployeeAsync(id, actingUserId);
            TempData["Success"] = "Employee approved.";
            return RedirectToAction(nameof(PendingApprovals));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Admin")]
        public async Task<IActionResult> Reject(int id, string? reason)
        {
            var actingUserId = HttpContext.Session.GetCurrentUserId() ?? 0;
            await _employeeService.RejectEmployeeAsync(id, actingUserId, reason);
            TempData["Success"] = "Application rejected.";
            return RedirectToAction(nameof(PendingApprovals));
        }

        private async Task PopulateFormDataAsync(int? selectedUserId = null, int? editingEmployeeId = null)
        {
            ViewBag.Users = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.IsActive && (!_context.Employees.Any(e => e.UserId == u.UserId && e.ApprovalStatus == "Approved") || u.UserId == selectedUserId))
                .ToListAsync();

            ViewBag.Departments = await _context.Departments.OrderBy(d => d.DepartmentName).ToListAsync();
            ViewBag.Managers = await _employeeService.GetManagersAsync(editingEmployeeId);
            ViewBag.Categories = await _context.Categories.OrderBy(c => c.CategoryName).ToListAsync();
            ViewBag.Shifts = new[] { "Morning", "Afternoon", "Night" };
        }

        private async Task PopulateOnboardFormDataAsync()
        {
            ViewBag.Departments = await _context.Departments.OrderBy(d => d.DepartmentName).ToListAsync();
            ViewBag.Managers = await _employeeService.GetManagersAsync();
            ViewBag.Categories = await _context.Categories.OrderBy(c => c.CategoryName).ToListAsync();
            ViewBag.Roles = await _context.Roles.ToListAsync();
            ViewBag.Shifts = new[] { "Morning", "Afternoon", "Night" };
        }
    }
}
