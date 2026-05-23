using System.Text.Json;
using InventoryManagementSystem.Attributes;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Helpers;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;
        private readonly IEmployeeService _employeeService;
        private readonly IInventoryService _inventoryService;

        public AuthController(
            IAuthService authService,
            ApplicationDbContext context,
            IEmployeeService employeeService,
            IInventoryService inventoryService)
        {
            _authService = authService;
            _context = context;
            _employeeService = employeeService;
            _inventoryService = inventoryService;
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString(SessionKeys.UserId) != null)
                return RedirectToAction("Index", "Dashboard");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Username and password are required");
                return View();
            }

            var user = await _authService.AuthenticateAsync(username, password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            HttpContext.Session.SetString(SessionKeys.UserId, user.UserId.ToString());
            HttpContext.Session.SetString(SessionKeys.Username, user.Username);
            HttpContext.Session.SetString(SessionKeys.UserRole, user.Role?.RoleName ?? "Employee");
            HttpContext.Session.SetString(SessionKeys.UserInfo, JsonSerializer.Serialize(new { user.UserId, user.Username, RoleName = user.Role?.RoleName }));

            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Register()
        {
            if (HttpContext.Session.GetString(SessionKeys.UserId) != null)
                return RedirectToAction("Index", "Dashboard");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "All fields are required");
                return View();
            }

            if (password != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match");
                return View();
            }

            if (password.Length < 6)
            {
                ModelState.AddModelError("", "Password must be at least 6 characters");
                return View();
            }

            if (await _authService.UsernameExistsAsync(username))
            {
                ModelState.AddModelError("", "Username already exists");
                return View();
            }

            if (await _authService.EmailExistsAsync(email))
            {
                ModelState.AddModelError("", "Email already exists");
                return View();
            }

            var user = await _authService.RegisterAsync(username, email, password, roleId: 2);
            if (user == null)
            {
                ModelState.AddModelError("", "Registration failed. Please try again.");
                return View();
            }

            HttpContext.Session.SetString(SessionKeys.UserId, user.UserId.ToString());
            HttpContext.Session.SetString(SessionKeys.Username, user.Username);
            HttpContext.Session.SetString(SessionKeys.UserRole, "Employee");

            TempData["Info"] = "Account created. Ask an admin to complete your employee profile, or use onboarding if you are an admin.";
            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetCurrentUserId();
            if (!userId.HasValue) return RedirectToAction("Login");

            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Employee)!.ThenInclude(e => e!.Department)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            var employee = await _employeeService.GetEmployeeByUserIdAsync(userId.Value);
            var transactions = await _inventoryService.GetByUserAsync(userId.Value, 10);

            ViewBag.User = user;
            ViewBag.Employee = employee;
            ViewBag.Transactions = transactions;
            return View();
        }
    }
}
