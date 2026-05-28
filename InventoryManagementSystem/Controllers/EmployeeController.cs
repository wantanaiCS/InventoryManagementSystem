using InventoryManagementSystem.Attributes;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Helpers;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.ViewModels;
using Microsoft.Extensions.Localization;
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
        private readonly IStringLocalizer<SharedResource> _l;

        public EmployeeController(IEmployeeService employeeService, ApplicationDbContext context, ILogger<EmployeeController> logger, IStringLocalizer<SharedResource> localizer)
        {
            _employeeService = employeeService;
            _context = context;
            _logger = logger;
            _l = localizer;
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
                ModelState.AddModelError(nameof(employee.UserId), _l["Employee.Error.UserRequired"]);

            if (await _employeeService.GetEmployeeByUserIdAsync(employee.UserId) != null)
                ModelState.AddModelError(nameof(employee.UserId), _l["Employee.Error.UserAlreadyHasEmployee"]);

            if (ModelState.IsValid)
            {
                try
                {
                    var actingUserId = HttpContext.Session.GetCurrentUserId() ?? 0;
                    await _employeeService.AddEmployeeAsync(employee, categoryIds ?? new List<int>(), actingUserId);
                    TempData["Success"] = _l["Employee.Success.Created"].Value;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Create employee failed");
                    ModelState.AddModelError(string.Empty, _l["Employee.Error.CreateFailed"]);
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
                    TempData["Success"] = string.Format(_l["Employee.Success.OnboardComplete"], employee.FullName);
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
                TempData["Success"] = _l["Employee.Success.Updated"].Value;
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
                TempData["Success"] = _l["Employee.Success.Deactivated"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Admin")]
        public async Task<IActionResult> ToggleActive(int id, bool isActive)
        {
            var actingUserId = HttpContext.Session.GetCurrentUserId() ?? 0;
            await _employeeService.SetActiveStatusAsync(id, isActive, actingUserId);
            TempData["Success"] = isActive ? _l["Employee.Success.Activated"].Value : _l["Employee.Success.DeactivatedSimple"].Value;
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
                    TempData["Info"] = _l["Employee.Info.PendingApproval"].Value;
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
                    TempData["Success"] = _l["Employee.Success.ApplicationSubmitted"].Value;
                    return RedirectToAction("Index", "Dashboard");
                }
                ModelState.AddModelError(string.Empty, error ?? _l["Employee.Error.SelfRegistrationFailed"]);
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
            TempData["Success"] = _l["Employee.Success.Approved"].Value;
            return RedirectToAction(nameof(PendingApprovals));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Admin")]
        public async Task<IActionResult> Reject(int id, string? reason)
        {
            var actingUserId = HttpContext.Session.GetCurrentUserId() ?? 0;
            await _employeeService.RejectEmployeeAsync(id, actingUserId, reason);
            TempData["Success"] = _l["Employee.Success.Rejected"].Value;
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
