using InventoryManagementSystem.Attributes;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Helpers;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public ProductController(IProductService productService, ApplicationDbContext context, IAuditService auditService)
        {
            _productService = productService;
            _context = context;
            _auditService = auditService;
        }

        public async Task<IActionResult> Index(string searchTerm = "")
        {
            var products = string.IsNullOrEmpty(searchTerm)
                ? await _productService.GetAllProductsAsync()
                : await _productService.SearchProductsAsync(searchTerm);
            ViewBag.SearchTerm = searchTerm;
            return View(products);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null) return NotFound();
            return View(product);
        }

        [Authorize("Admin")]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Admin")]
        public async Task<IActionResult> Create([Bind("ProductCode,ProductName,CategoryId,Price,CurrentStock,Description")] Product product)
        {
            if (await _productService.GetProductByCodeAsync(product.ProductCode) != null)
                ModelState.AddModelError("ProductCode", "Product Code already exists");

            if (ModelState.IsValid)
            {
                product.CreatedDate = DateTime.Now;
                await _productService.AddProductAsync(product);
                var userId = HttpContext.Session.GetCurrentUserId() ?? 0;
                await _auditService.LogAsync(userId, "CREATE", "Products", product.ProductId);
                TempData["Success"] = "Product created.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        [Authorize("Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null) return NotFound();
            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductCode,ProductName,CategoryId,Price,CurrentStock,Description,CreatedDate")] Product product)
        {
            if (id != product.ProductId) return NotFound();

            var existing = await _productService.GetProductByCodeAsync(product.ProductCode);
            if (existing != null && existing.ProductId != id)
                ModelState.AddModelError("ProductCode", "Product Code already exists");

            if (ModelState.IsValid)
            {
                await _productService.UpdateProductAsync(product);
                var userId = HttpContext.Session.GetCurrentUserId() ?? 0;
                await _auditService.LogAsync(userId, "UPDATE", "Products", product.ProductId);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        [Authorize("Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize("Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteProductAsync(id);
            var userId = HttpContext.Session.GetCurrentUserId() ?? 0;
            await _auditService.LogAsync(userId, "DELETE", "Products", id);
            return RedirectToAction(nameof(Index));
        }
    }
}
