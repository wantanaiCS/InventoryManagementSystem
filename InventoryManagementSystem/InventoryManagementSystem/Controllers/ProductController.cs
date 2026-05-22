using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Attributes;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ApplicationDbContext _context;

        public ProductController(IProductService productService, ApplicationDbContext context)
        {
            _productService = productService;
            _context = context;
        }

        // GET: Product/Index
        public async Task<IActionResult> Index(string searchTerm = "")
        {
            IEnumerable<Product> products;

            if (string.IsNullOrEmpty(searchTerm))
            {
                products = await _productService.GetAllProductsAsync();
            }
            else
            {
                products = await _productService.SearchProductsAsync(searchTerm);
            }

            ViewBag.SearchTerm = searchTerm;
            return View(products);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductCode,ProductName,CategoryId,Price,CurrentStock,Description")] Product product)
        {
            // ตรวจสอบว่า ProductCode ซ้ำหรือไม่
            var existingProduct = await _productService.GetProductByCodeAsync(product.ProductCode);
            if (existingProduct != null)
            {
                ModelState.AddModelError("ProductCode", "Product Code already exists");
            }

            if (ModelState.IsValid)
            {
                product.CreatedDate = DateTime.Now;
                await _productService.AddProductAsync(product);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductCode,ProductName,CategoryId,Price,CurrentStock,Description,CreatedDate")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            // ตรวจสอบ ProductCode ซ้ำ (ยกเว้นตัวเองที่กำลังแก้ไข)
            var existingProduct = await _productService.GetProductByCodeAsync(product.ProductCode);
            if (existingProduct != null && existingProduct.ProductId != id)
            {
                ModelState.AddModelError("ProductCode", "Product Code already exists");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.UpdateProductAsync(product);
                }
                catch (Exception)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
