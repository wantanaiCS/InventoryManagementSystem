namespace InventoryManagementSystem.Services
{
    public interface IAuditService
    {
        Task LogAsync(int userId, string action, string tableName, int recordId, string? oldValues = null, string? newValues = null);
        Task<IReadOnlyList<Models.AuditLog>> GetForRecordAsync(string tableName, int recordId, int take = 20);
        Task<IReadOnlyList<Models.AuditLog>> GetForUserAsync(int userId, int take = 20);
    }
}
