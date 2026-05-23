using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(int userId, string title, string message, string type = "info")
        {
            _context.AppNotifications.Add(new AppNotification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        public async Task NotifyAdminsAsync(string title, string message, string type = "info")
        {
            var adminUserIds = await _context.Users
                .Where(u => u.RoleId == 1 && u.IsActive)
                .Select(u => u.UserId)
                .ToListAsync();

            foreach (var userId in adminUserIds)
            {
                await CreateAsync(userId, title, message, type);
            }
        }

        public async Task<IReadOnlyList<AppNotification>> GetUnreadAsync(int userId, int take = 10)
        {
            return await _context.AppNotifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _context.AppNotifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RunSystemChecksAsync()
        {
            var adminIds = await _context.Users.Where(u => u.RoleId == 1 && u.IsActive).Select(u => u.UserId).ToListAsync();
            if (adminIds.Count == 0) return;

            var lowStock = await _context.Products.CountAsync(p => p.CurrentStock <= 5);
            if (lowStock > 0)
            {
                foreach (var adminId in adminIds)
                {
                    var exists = await _context.AppNotifications.AnyAsync(n =>
                        n.UserId == adminId && !n.IsRead && n.Title == "Low stock alert");
                    if (!exists)
                    {
                        await CreateAsync(adminId, "Low stock alert",
                            $"{lowStock} product(s) have stock at or below 5 units.", "warning");
                    }
                }
            }

            var usersWithoutEmployee = await _context.Users
                .CountAsync(u => u.IsActive && u.RoleId == 2 && !_context.Employees.Any(e => e.UserId == u.UserId));

            if (usersWithoutEmployee > 0)
            {
                foreach (var adminId in adminIds)
                {
                    var exists = await _context.AppNotifications.AnyAsync(n =>
                        n.UserId == adminId && !n.IsRead && n.Title == "Incomplete onboarding");
                    if (!exists)
                    {
                        await CreateAsync(adminId, "Incomplete onboarding",
                            $"{usersWithoutEmployee} user account(s) need employee profiles.", "info");
                    }
                }
            }
        }
    }
}
