namespace InventoryManagementSystem.Services
{
    public interface IExportService
    {
        Task<byte[]> ExportEmployeesAsync();
        Task<byte[]> ExportInventoryHistoryAsync();
    }
}
