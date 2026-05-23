using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Attributes;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Controllers
{
    [Authorize] // Allow all logged-in users
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

        // GET: Employee/Index
        public async Task<IActionResult> Index(string searchTerm = "")
        {
            IEnumerable<Employee> employees;

            if (string.IsNullOrEmpty(searchTerm))
            {
                employees = await _employeeService.GetAllEmployeesAsync();
            }
            else
            {
                employees = await _employeeService.SearchEmployeesAsync(searchTerm);
            }

            ViewBag.SearchTerm = searchTerm;
            return View(employees);
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            // Get users that don't have employee records yet
            var usersWithoutEmployee = _context.Users
                .Include(u => u.Role)
                .Where(u => !_context.Employees.Any(e => e.UserId == u.UserId))
                .ToList();

            ViewBag.Users = usersWithoutEmployee;
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,FullName,Position,HireDate,PhoneNumber,Address")] Employee employee)
        {
            if (employee.UserId <= 0)
            {
                ModelState.AddModelError(nameof(employee.UserId), "Please select a user account.");
            }

            // Check if employee already exists for this user
            var existingEmployee = await _employeeService.GetEmployeeByUserIdAsync(employee.UserId);
            if (existingEmployee != null)
            {
                ModelState.AddModelError("UserId", "This user already has an employee record");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("Creating employee for UserId={UserId}", employee.UserId);
                    await _employeeService.AddEmployeeAsync(employee);
                    _logger.LogInformation("Employee created: EmployeeId={EmployeeId}", employee.EmployeeId);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating employee for UserId={UserId}", employee.UserId);
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the employee. Please try again and check server logs.");
                }
            }

            // Log ModelState errors for debugging
            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    if (entry.Value?.Errors != null && entry.Value.Errors.Count > 0)
                    {
                        foreach (var err in entry.Value.Errors)
                        {
                            _logger.LogWarning("ModelState error on {Key}: {Error}", entry.Key, err.ErrorMessage);
                        }
                    }
                }
            }

            var usersWithoutEmployee = _context.Users
                .Include(u => u.Role)
                .Where(u => !_context.Employees.Any(e => e.UserId == u.UserId))
                .ToList();
            ViewBag.Users = usersWithoutEmployee;
            return View(employee);
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,UserId,FullName,Position,HireDate,PhoneNumber,Address")] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _employeeService.UpdateEmployeeAsync(employee);
                }
                catch (Exception)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }

        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _employeeService.DeleteEmployeeAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
