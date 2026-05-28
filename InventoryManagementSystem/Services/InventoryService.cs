using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace InventoryManagementSystem.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResource> _l;

        public InventoryService(ApplicationDbContext context, IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _l = localizer;
        }

        public async Task<(bool Success, string? Error)> RecordTransactionAsync(
            int productId, string transactionType, int quantity, int userId, string notes, string shift)
        {
            transactionType = transactionType.ToUpperInvariant();
            if (transactionType is not ("IN" or "OUT"))
                return (false, _l["Inventory.Error.TypeInvalid"]);

            if (quantity <= 0)
                return (false, _l["Inventory.Error.QuantityInvalid"]);

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
                return (false, _l["Inventory.Error.ProductNotFound"]);

            if (transactionType == "OUT" && product.CurrentStock < quantity)
                return (false, string.Format(_l["Inventory.Error.InsufficientStock"], product.CurrentStock));

            var employee = await _context.Employees
                .Include(e => e.CategoryAssignments)
                .FirstOrDefaultAsync(e => e.UserId == userId && !e.IsDeleted);

            if (employee != null && employee.ApprovalStatus != "Approved")
                return (false, _l["Inventory.Error.EmployeePending"]);

            if (employee != null && employee.CategoryAssignments.Count > 0)
            {
                var allowed = employee.CategoryAssignments.Any(a => a.CategoryId == product.CategoryId);
                if (!allowed)
                    return (false, _l["Inventory.Error.CategoryNotAssigned"]);
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
