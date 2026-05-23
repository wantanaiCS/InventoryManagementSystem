using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Services
{
    public interface INotificationService
    {
        Task CreateAsync(int userId, string title, string message, string type = "info");
        Task NotifyAdminsAsync(string title, string message, string type = "info");
        Task<IReadOnlyList<AppNotification>> GetUnreadAsync(int userId, int take = 10);
        Task MarkAsReadAsync(int notificationId, int userId);
        Task RunSystemChecksAsync();
    }
}
