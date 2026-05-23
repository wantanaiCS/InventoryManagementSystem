using InventoryManagementSystem.Attributes;
using InventoryManagementSystem.Helpers;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly INotificationService _notificationService;

        public DashboardController(IDashboardService dashboardService, INotificationService notificationService)
        {
            _dashboardService = dashboardService;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetCurrentUserId();
            if (!userId.HasValue) return RedirectToAction("Login", "Auth");

            await _notificationService.RunSystemChecksAsync();
            var vm = await _dashboardService.GetDashboardAsync(userId.Value, HttpContext.Session.IsAdmin());
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkNotificationRead(int id)
        {
            var userId = HttpContext.Session.GetCurrentUserId();
            if (userId.HasValue)
                await _notificationService.MarkAsReadAsync(id, userId.Value);
            return RedirectToAction(nameof(Index));
        }
    }
}
