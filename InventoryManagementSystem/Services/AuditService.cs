using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Services
{
    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;

        public AuditService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(int userId, string action, string tableName, int recordId, string? oldValues = null, string? newValues = null)
        {
            _context.AuditLogs.Add(new AuditLog
            {
                UserId = userId,
                Action = action,
                TableName = tableName,
                RecordId = recordId,
                OldValues = oldValues ?? string.Empty,
                NewValues = newValues ?? string.Empty,
                Timestamp = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<AuditLog>> GetForRecordAsync(string tableName, int recordId, int take = 20)
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.TableName == tableName && a.RecordId == recordId)
                .OrderByDescending(a => a.Timestamp)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<AuditLog>> GetForUserAsync(int userId, int take = 20)
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
