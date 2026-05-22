using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;
using System.Text.Json;

namespace InventoryManagementSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // GET: Auth/Login
        public IActionResult Login()
        {
            // ถ้ามี session อยู่ให้ redirect ไป home
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: Auth/Login
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

            // Set session
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("UserRole", user.Role?.RoleName ?? "Employee");

            // Set user info as JSON for easy access
            var userInfo = new { user.UserId, user.Username, RoleName = user.Role?.RoleName };
            HttpContext.Session.SetString("UserInfo", JsonSerializer.Serialize(userInfo));

            return RedirectToAction("Index", "Home");
        }

        // GET: Auth/Register
        public IActionResult Register()
        {
            // ถ้ามี session อยู่ให้ redirect ไป home
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string email, string password, string confirmPassword)
        {
            // Validation
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

            // Register user (default role is Employee = 2)
            var user = await _authService.RegisterAsync(username, email, password, roleId: 2);

            if (user == null)
            {
                ModelState.AddModelError("", "Registration failed. Please try again.");
                return View();
            }

            // Set session
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("UserRole", "Employee");

            return RedirectToAction("Index", "Home");
        }

        // GET: Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: Auth/Profile (view current user info)
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("UserRole");

            ViewBag.Username = username;
            ViewBag.Role = role;

            return View();
        }
    }
}
