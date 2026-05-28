using InventoryManagementSystem.Attributes;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Helpers;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;
        private readonly IStringLocalizer<SharedResource> _l;

        private const string FurnitureProductLine = "Furniture";

        public ProductController(IProductService productService, ApplicationDbContext context, IAuditService auditService, IStringLocalizer<SharedResource> localizer)
        {
            _productService = productService;
            _context = context;
            _auditService = auditService;
            _l = localizer;
        }

        private async Task<List<Category>> GetFurnitureCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.ProductLine == FurnitureProductLine)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<IActionResult> Index(string searchTerm = "", int? categoryId = null)
        {
            var products = string.IsNullOrEmpty(searchTerm)
                ? await _productService.GetAllProductsAsync()
                : await _productService.SearchProductsAsync(searchTerm);

            if (categoryId.HasValue)
                products = products.Where(p => p.CategoryId == categoryId.Value);

            ViewBag.SearchTerm = searchTerm;
            ViewBag.CategoryId = categoryId;
            ViewBag.Categories = await GetFurnitureCategoriesAsync();
            ViewBag.IsAdmin = HttpContext.Session.IsAdmin();
            return View(products);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null) return NotFound();
            ViewBag.IsAdmin = HttpContext.Session.IsAdmin();
            return View(product);
        }

        [Authorize("Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await GetFurnitureCategoriesAsync();
            return View(new Product { Unit = "ชิ้น", CurrentStock = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Admin")]
        public async Task<IActionResult> Create([Bind("ProductCode,ProductName,CategoryId,Price,CurrentStock,Description,Material,Color,WidthCm,DepthCm,HeightCm,Unit,WarehouseLocation")] Product product)
        {
            if (await _productService.GetProductByCodeAsync(product.ProductCode) != null)
                ModelState.AddModelError("ProductCode", _l["Product.Error.CodeExists"]);

            if (product.CategoryId <= 0)
                ModelState.AddModelError("CategoryId", _l["Product.Error.CategoryRequired"]);

            if (ModelState.IsValid)
            {
                product.CreatedDate = DateTime.Now;
                await _productService.AddProductAsync(product);
                var userId = HttpContext.Session.GetCurrentUserId() ?? 0;
                await _auditService.LogAsync(userId, "CREATE", "Products", product.ProductId);
                TempData["Success"] = _l["Product.Success.Created"].Value;
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await GetFurnitureCategoriesAsync();
            return View(product);
        }

        [Authorize("Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null) return NotFound();
            ViewBag.Categories = await GetFurnitureCategoriesAsync();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductCode,ProductName,CategoryId,Price,CurrentStock,Description,CreatedDate,Material,Color,WidthCm,DepthCm,HeightCm,Unit,WarehouseLocation")] Product product)
        {
            if (id != product.ProductId) return NotFound();

            var existing = await _productService.GetProductByCodeAsync(product.ProductCode);
            if (existing != null && existing.ProductId != id)
                ModelState.AddModelError("ProductCode", _l["Product.Error.CodeExists"]);

            if (ModelState.IsValid)
            {
                await _productService.UpdateProductAsync(product);
                var userId = HttpContext.Session.GetCurrentUserId() ?? 0;
                await _auditService.LogAsync(userId, "UPDATE", "Products", product.ProductId);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await GetFurnitureCategoriesAsync();
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
