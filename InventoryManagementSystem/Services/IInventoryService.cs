using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Services
{
    public interface IInventoryService
    {
        Task<(bool Success, string? Error)> RecordTransactionAsync(int productId, string transactionType, int quantity, int userId, string notes, string shift);
        Task<IReadOnlyList<InventoryTransaction>> GetRecentAsync(int take = 20);
        Task<IReadOnlyList<InventoryTransaction>> GetByUserAsync(int userId, int take = 50);
        Task<IReadOnlyList<InventoryTransaction>> GetByProductAsync(int productId, int take = 50);
        Task<PagedResult<InventoryTransaction>> GetPagedHistoryAsync(int page, int pageSize, string? type = null);
    }
}
