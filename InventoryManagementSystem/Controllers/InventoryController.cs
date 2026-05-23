using InventoryManagementSystem.Attributes;
using InventoryManagementSystem.Helpers;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Data;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public InventoryController(IInventoryService inventoryService, ApplicationDbContext context, IAuditService auditService)
        {
            _inventoryService = inventoryService;
            _context = context;
            _auditService = auditService;
        }

        public async Task<IActionResult> Index(int page = 1, string? type = null)
        {
            var history = await _inventoryService.GetPagedHistoryAsync(page, 15, type);
            ViewBag.Type = type;
            return View(history);
        }

        public async Task<IActionResult> Receive()
        {
            ViewBag.Products = await _context.Products.Include(p => p.Category).OrderBy(p => p.ProductName).ToListAsync();
            ViewBag.Shifts = new[] { "Morning", "Afternoon", "Night" };
            return View(new InventoryTransactionFormViewModel { Shift = "Morning" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Receive(InventoryTransactionFormViewModel model)
        {
            var userId = HttpContext.Session.GetCurrentUserId();
            if (!userId.HasValue) return RedirectToAction("Login", "Auth");

            var (success, error) = await _inventoryService.RecordTransactionAsync(
                model.ProductId, "IN", model.Quantity, userId.Value, model.Notes, model.Shift);

            if (success)
            {
                await _auditService.LogAsync(userId.Value, "STOCK_IN", "InventoryTransactions", model.ProductId);
                TempData["Success"] = "Stock received successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, error ?? "Transaction failed.");
            ViewBag.Products = await _context.Products.Include(p => p.Category).OrderBy(p => p.ProductName).ToListAsync();
            ViewBag.Shifts = new[] { "Morning", "Afternoon", "Night" };
            return View(model);
        }

        public async Task<IActionResult> Dispense()
        {
            ViewBag.Products = await _context.Products.Include(p => p.Category).OrderBy(p => p.ProductName).ToListAsync();
            ViewBag.Shifts = new[] { "Morning", "Afternoon", "Night" };
            return View(new InventoryTransactionFormViewModel { Shift = "Morning" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dispense(InventoryTransactionFormViewModel model)
        {
            var userId = HttpContext.Session.GetCurrentUserId();
            if (!userId.HasValue) return RedirectToAction("Login", "Auth");

            var (success, error) = await _inventoryService.RecordTransactionAsync(
                model.ProductId, "OUT", model.Quantity, userId.Value, model.Notes, model.Shift);

            if (success)
            {
                await _auditService.LogAsync(userId.Value, "STOCK_OUT", "InventoryTransactions", model.ProductId);
                TempData["Success"] = "Stock dispensed successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, error ?? "Transaction failed.");
            ViewBag.Products = await _context.Products.Include(p => p.Category).OrderBy(p => p.ProductName).ToListAsync();
            ViewBag.Shifts = new[] { "Morning", "Afternoon", "Night" };
            return View(model);
        }
    }
}
