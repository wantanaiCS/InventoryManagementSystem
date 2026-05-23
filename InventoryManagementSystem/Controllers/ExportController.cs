using InventoryManagementSystem.Attributes;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    [Authorize("Admin")]
    public class ExportController : Controller
    {
        private readonly IExportService _exportService;

        public ExportController(IExportService exportService)
        {
            _exportService = exportService;
        }

        public async Task<IActionResult> Employees()
        {
            var bytes = await _exportService.ExportEmployeesAsync();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"employees_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        public async Task<IActionResult> InventoryHistory()
        {
            var bytes = await _exportService.ExportInventoryHistoryAsync();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"inventory_{DateTime.Now:yyyyMMdd}.xlsx");
        }
    }
}
