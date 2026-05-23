using System.Text.Json;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;
        private readonly INotificationService _notificationService;
        private readonly IAuthService _authService;

        public EmployeeService(
            ApplicationDbContext context,
            IAuditService auditService,
            INotificationService notificationService,
            IAuthService authService)
        {
            _context = context;
            _auditService = auditService;
            _notificationService = notificationService;
            _authService = authService;
        }

        private IQueryable<Employee> Query(bool includeDeleted)
        {
            var q = _context.Employees.AsQueryable();
            if (includeDeleted)
                q = q.IgnoreQueryFilters();
            return q;
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id, bool includeDeleted = false)
        {
            return await Query(includeDeleted)
                .Include(e => e.User)!.ThenInclude(u => u!.Role)
                .Include(e => e.Department)
                .Include(e => e.ReportsTo)
                .Include(e => e.CategoryAssignments).ThenInclude(c => c.Category)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }

        public async Task<EmployeeDetailsViewModel?> GetEmployeeDetailsAsync(int id)
        {
            var employee = await GetEmployeeByIdAsync(id);
            if (employee == null) return null;

            var userId = employee.UserId;
            var today = DateTime.Today;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);

            var transactions = await _context.InventoryTransactions
                .Include(t => t.Product)
                .Where(t => t.CreatedBy == userId)
                .OrderByDescending(t => t.CreatedDate)
                .Take(50)
                .AsNoTracking()
                .ToListAsync();

            var audit = await _auditService.GetForRecordAsync("Employees", id, 15);

            var timeline = new List<ActivityItem>();

            foreach (var t in transactions.Take(10))
            {
                timeline.Add(new ActivityItem
                {
                    Timestamp = t.CreatedDate,
                    Title = $"{t.TransactionType} stock",
                    Description = $"{t.Quantity} × {t.Product?.ProductName}",
                    Icon = t.TransactionType == "IN" ? "fa-arrow-down" : "fa-arrow-up",
                    BadgeClass = t.TransactionType == "IN" ? "bg-success" : "bg-warning"
                });
            }

            foreach (var a in audit)
            {
                timeline.Add(new ActivityItem
                {
                    Timestamp = a.Timestamp,
                    Title = $"Record {a.Action}",
                    Description = $"Employees #{a.RecordId} by {a.User?.Username}",
                    Icon = "fa-clipboard-list",
                    BadgeClass = "bg-info"
                });
            }

            timeline = timeline.OrderByDescending(x => x.Timestamp).Take(15).ToList();

            return new EmployeeDetailsViewModel
            {
                Employee = employee,
                RecentTransactions = transactions.Take(20).ToList(),
                AuditTrail = audit.ToList(),
                TransactionsToday = transactions.Count(t => t.CreatedDate.Date == today),
                TransactionsThisWeek = transactions.Count(t => t.CreatedDate >= weekStart),
                TotalInQuantity = transactions.Where(t => t.TransactionType == "IN").Sum(t => t.Quantity),
                TotalOutQuantity = transactions.Where(t => t.TransactionType == "OUT").Sum(t => t.Quantity),
                ActivityTimeline = timeline
            };
        }

        public async Task<PagedResult<Employee>> GetPagedAsync(EmployeeFilterViewModel filter)
        {
            filter.Page = Math.Max(1, filter.Page);
            filter.PageSize = Math.Clamp(filter.PageSize, 5, 50);

            var query = Query(false)
                .Include(e => e.User)!.ThenInclude(u => u!.Role)
                .Include(e => e.Department)
                .Where(e => e.ApprovalStatus == "Approved")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim();
                query = query.Where(e =>
                    e.FullName.Contains(term) ||
                    e.Position.Contains(term) ||
                    (e.User != null && e.User.Username.Contains(term)));
            }

            if (filter.DepartmentId.HasValue)
                query = query.Where(e => e.DepartmentId == filter.DepartmentId);

            if (!string.IsNullOrWhiteSpace(filter.Position))
                query = query.Where(e => e.Position.Contains(filter.Position));

            if (filter.IsActive.HasValue)
                query = query.Where(e => e.IsActive == filter.IsActive);

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(e => e.FullName)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<Employee> { Items = items, Page = filter.Page, PageSize = filter.PageSize, TotalCount = total };
        }

        public async Task<Employee?> GetEmployeeByUserIdAsync(int userId)
        {
            return await Query(false)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }

        public async Task AddEmployeeAsync(Employee employee, IEnumerable<int> categoryIds, int actingUserId)
        {
            employee.RegistrationSource = "Admin";
            employee.ApprovalStatus = "Approved";
            employee.IsActive = true;
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            await SetCategoryAssignmentsAsync(employee.EmployeeId, categoryIds);
            await SyncUserActiveAsync(employee.UserId, employee.IsActive);
            await _auditService.LogAsync(actingUserId, "CREATE", "Employees", employee.EmployeeId,
                newValues: JsonSerializer.Serialize(new { employee.FullName, employee.UserId }));
        }

        public async Task UpdateEmployeeAsync(Employee employee, IEnumerable<int> categoryIds, int actingUserId)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            await SetCategoryAssignmentsAsync(employee.EmployeeId, categoryIds);
            await SyncUserActiveAsync(employee.UserId, employee.IsActive);
            await _auditService.LogAsync(actingUserId, "UPDATE", "Employees", employee.EmployeeId,
                newValues: JsonSerializer.Serialize(new { employee.FullName, employee.IsActive }));
        }

        public async Task<(bool Success, string? Error)> SoftDeleteAsync(int id, int actingUserId)
        {
            var employee = await Query(true).Include(e => e.User).FirstOrDefaultAsync(e => e.EmployeeId == id);
            if (employee == null) return (false, "Employee not found.");

            var hasTransactions = await _context.InventoryTransactions.AnyAsync(t => t.CreatedBy == employee.UserId);
            if (hasTransactions)
            {
                employee.IsDeleted = true;
                employee.IsActive = false;
                if (employee.User != null) employee.User.IsActive = false;
                await _context.SaveChangesAsync();
                await _auditService.LogAsync(actingUserId, "SOFT_DELETE", "Employees", id);
                return (true, null);
            }

            employee.IsDeleted = true;
            employee.IsActive = false;
            if (employee.User != null) employee.User.IsActive = false;
            await _context.SaveChangesAsync();
            await _auditService.LogAsync(actingUserId, "DELETE", "Employees", id);
            return (true, null);
        }

        public async Task SetActiveStatusAsync(int id, bool isActive, int actingUserId)
        {
            var employee = await GetEmployeeByIdAsync(id) ?? throw new InvalidOperationException("Employee not found");
            employee.IsActive = isActive;
            await SyncUserActiveAsync(employee.UserId, isActive);
            await _context.SaveChangesAsync();
            await _auditService.LogAsync(actingUserId, isActive ? "ACTIVATE" : "DEACTIVATE", "Employees", id);
        }

        public async Task<Employee> OnboardAsync(EmployeeOnboardViewModel model, int actingUserId)
        {
            var user = await _authService.RegisterAsync(model.Username, model.Email, model.Password, model.RoleId)
                ?? throw new InvalidOperationException("Could not create user account.");

            var employee = new Employee
            {
                UserId = user.UserId,
                FullName = model.FullName,
                Position = model.Position,
                HireDate = model.HireDate,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                DepartmentId = model.DepartmentId,
                ReportsToEmployeeId = model.ReportsToEmployeeId,
                Shift = model.Shift,
                IsActive = true,
                RegistrationSource = "Admin",
                ApprovalStatus = "Approved"
            };

            await AddEmployeeAsync(employee, model.CategoryIds, actingUserId);
            await _notificationService.NotifyAdminsAsync("New employee onboarded",
                $"{employee.FullName} joined as {employee.Position}.", "success");
            return employee;
        }

        public async Task<IReadOnlyList<Employee>> GetManagersAsync(int? excludeEmployeeId = null)
        {
            var query = Query(false).Where(e => e.IsActive);
            if (excludeEmployeeId.HasValue)
                query = query.Where(e => e.EmployeeId != excludeEmployeeId);
            return await query.OrderBy(e => e.FullName).AsNoTracking().ToListAsync();
        }

        private async Task SetCategoryAssignmentsAsync(int employeeId, IEnumerable<int> categoryIds)
        {
            var existing = await _context.EmployeeCategoryAssignments
                .Where(x => x.EmployeeId == employeeId)
                .ToListAsync();
            _context.EmployeeCategoryAssignments.RemoveRange(existing);

            foreach (var categoryId in categoryIds.Distinct())
            {
                _context.EmployeeCategoryAssignments.Add(new EmployeeCategoryAssignment
                {
                    EmployeeId = employeeId,
                    CategoryId = categoryId
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<(bool Success, string? Error)> ApplySelfRegistrationAsync(int userId, EmployeeSelfRegisterViewModel model)
        {
            if (await GetEmployeeByUserIdAsync(userId) != null)
                return (false, "You already have an employee profile.");

            var pending = await _context.Employees.IgnoreQueryFilters()
                .AnyAsync(e => e.UserId == userId && e.ApprovalStatus == "Pending" && !e.IsDeleted);
            if (pending)
                return (false, "Your application is already pending admin approval.");

            var employee = new Employee
            {
                UserId = userId,
                FullName = model.FullName,
                Position = model.Position,
                HireDate = model.HireDate,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                DepartmentId = model.DepartmentId,
                Shift = model.Shift,
                IsActive = false,
                RegistrationSource = "SelfService",
                ApprovalStatus = "Pending"
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            await _notificationService.NotifyAdminsAsync("Employee registration pending",
                $"{model.FullName} submitted a self-registration request.", "warning");
            return (true, null);
        }

        public async Task<IReadOnlyList<Employee>> GetPendingApprovalsAsync()
        {
            return await _context.Employees.IgnoreQueryFilters()
                .Include(e => e.User)
                .Include(e => e.Department)
                .Where(e => !e.IsDeleted && e.ApprovalStatus == "Pending")
                .OrderBy(e => e.HireDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task ApproveEmployeeAsync(int employeeId, int actingUserId)
        {
            var employee = await _context.Employees.IgnoreQueryFilters()
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId)
                ?? throw new InvalidOperationException("Employee not found");

            employee.ApprovalStatus = "Approved";
            employee.IsActive = true;
            if (employee.User != null) employee.User.IsActive = true;
            await _context.SaveChangesAsync();
            await _auditService.LogAsync(actingUserId, "APPROVE", "Employees", employeeId);

            if (employee.User != null)
            {
                await _notificationService.CreateAsync(employee.UserId, "Registration approved",
                    "Your employee profile has been approved. You can now use warehouse features.", "success");
            }
        }

        public async Task RejectEmployeeAsync(int employeeId, int actingUserId, string? reason = null)
        {
            var employee = await _context.Employees.IgnoreQueryFilters()
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId)
                ?? throw new InvalidOperationException("Employee not found");

            employee.ApprovalStatus = "Rejected";
            employee.IsActive = false;
            await _context.SaveChangesAsync();
            await _auditService.LogAsync(actingUserId, "REJECT", "Employees", employeeId,
                newValues: reason ?? "Rejected");

            if (employee.User != null)
            {
                await _notificationService.CreateAsync(employee.UserId, "Registration rejected",
                    reason ?? "Your employee registration was not approved. Contact HR.", "warning");
            }
        }

        private async Task SyncUserActiveAsync(int userId, bool isActive)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsActive = isActive;
                await _context.SaveChangesAsync();
            }
        }
    }
}
