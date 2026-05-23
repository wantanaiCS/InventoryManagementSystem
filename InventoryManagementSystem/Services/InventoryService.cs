using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string? Error)> RecordTransactionAsync(
            int productId, string transactionType, int quantity, int userId, string notes, string shift)
        {
            transactionType = transactionType.ToUpperInvariant();
            if (transactionType is not ("IN" or "OUT"))
                return (false, "Transaction type must be IN or OUT.");

            if (quantity <= 0)
                return (false, "Quantity must be greater than zero.");

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
                return (false, "Product not found.");

            if (transactionType == "OUT" && product.CurrentStock < quantity)
                return (false, $"Insufficient stock. Available: {product.CurrentStock}");

            var employee = await _context.Employees
                .Include(e => e.CategoryAssignments)
                .FirstOrDefaultAsync(e => e.UserId == userId && !e.IsDeleted);

            if (employee != null && employee.ApprovalStatus != "Approved")
                return (false, "Your employee profile is pending approval. Please wait for admin.");

            if (employee != null && employee.CategoryAssignments.Count > 0)
            {
                var allowed = employee.CategoryAssignments.Any(a => a.CategoryId == product.CategoryId);
                if (!allowed)
                    return (false, "You are not assigned to manage this product category.");
            }

            if (transactionType == "IN")
                product.CurrentStock += quantity;
            else
                product.CurrentStock -= quantity;

            var transaction = new InventoryTransaction
            {
                ProductId = productId,
                TransactionType = transactionType,
                Quantity = quantity,
                CreatedBy = userId,
                CreatedDate = DateTime.Now,
                Notes = notes,
                Shift = shift
            };

            _context.InventoryTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<IReadOnlyList<InventoryTransaction>> GetRecentAsync(int take = 20)
        {
            return await _context.InventoryTransactions
                .Include(t => t.Product)
                .Include(t => t.CreatedByUser)
                .OrderByDescending(t => t.CreatedDate)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<InventoryTransaction>> GetByUserAsync(int userId, int take = 50)
        {
            return await _context.InventoryTransactions
                .Include(t => t.Product)
                .Where(t => t.CreatedBy == userId)
                .OrderByDescending(t => t.CreatedDate)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<InventoryTransaction>> GetByProductAsync(int productId, int take = 50)
        {
            return await _context.InventoryTransactions
                .Include(t => t.CreatedByUser)
                .Where(t => t.ProductId == productId)
                .OrderByDescending(t => t.CreatedDate)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PagedResult<InventoryTransaction>> GetPagedHistoryAsync(int page, int pageSize, string? type = null)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 5, 50);

            var query = _context.InventoryTransactions
                .Include(t => t.Product)
                .Include(t => t.CreatedByUser)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(t => t.TransactionType == type.ToUpperInvariant());

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(t => t.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<InventoryTransaction>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };
        }
    }
}
