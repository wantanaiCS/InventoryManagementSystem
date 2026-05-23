using ClosedXML.Excel;
using InventoryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Services
{
    public class ExportService : IExportService
    {
        private readonly ApplicationDbContext _context;

        public ExportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<byte[]> ExportEmployeesAsync()
        {
            var employees = await _context.Employees
                .Include(e => e.User)!.ThenInclude(u => u!.Role)
                .Include(e => e.Department)
                .Include(e => e.ReportsTo)
                .OrderBy(e => e.FullName)
                .AsNoTracking()
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Employees");
            sheet.Cell(1, 1).Value = "Full Name";
            sheet.Cell(1, 2).Value = "Position";
            sheet.Cell(1, 3).Value = "Department";
            sheet.Cell(1, 4).Value = "Shift";
            sheet.Cell(1, 5).Value = "Status";
            sheet.Cell(1, 6).Value = "Username";
            sheet.Cell(1, 7).Value = "Role";
            sheet.Cell(1, 8).Value = "Manager";
            sheet.Cell(1, 9).Value = "Hire Date";
            sheet.Cell(1, 10).Value = "Phone";

            var row = 2;
            foreach (var e in employees)
            {
                sheet.Cell(row, 1).Value = e.FullName;
                sheet.Cell(row, 2).Value = e.Position;
                sheet.Cell(row, 3).Value = e.Department?.DepartmentName ?? "";
                sheet.Cell(row, 4).Value = e.Shift;
                sheet.Cell(row, 5).Value = e.IsActive ? "Active" : "Inactive";
                sheet.Cell(row, 6).Value = e.User?.Username ?? "";
                sheet.Cell(row, 7).Value = e.User?.Role?.RoleName ?? "";
                sheet.Cell(row, 8).Value = e.ReportsTo?.FullName ?? "";
                sheet.Cell(row, 9).Value = e.HireDate;
                sheet.Cell(row, 10).Value = e.PhoneNumber;
                row++;
            }

            sheet.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> ExportInventoryHistoryAsync()
        {
            var transactions = await _context.InventoryTransactions
                .Include(t => t.Product)
                .Include(t => t.CreatedByUser)
                .OrderByDescending(t => t.CreatedDate)
                .Take(5000)
                .AsNoTracking()
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Inventory History");
            sheet.Cell(1, 1).Value = "Date";
            sheet.Cell(1, 2).Value = "Type";
            sheet.Cell(1, 3).Value = "Product";
            sheet.Cell(1, 4).Value = "Quantity";
            sheet.Cell(1, 5).Value = "Shift";
            sheet.Cell(1, 6).Value = "By";
            sheet.Cell(1, 7).Value = "Notes";

            var row = 2;
            foreach (var t in transactions)
            {
                sheet.Cell(row, 1).Value = t.CreatedDate;
                sheet.Cell(row, 2).Value = t.TransactionType;
                sheet.Cell(row, 3).Value = t.Product?.ProductName ?? "";
                sheet.Cell(row, 4).Value = t.Quantity;
                sheet.Cell(row, 5).Value = t.Shift;
                sheet.Cell(row, 6).Value = t.CreatedByUser?.Username ?? "";
                sheet.Cell(row, 7).Value = t.Notes;
                row++;
            }

            sheet.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
