using InventoryManagementSystem.Attributes;
using InventoryManagementSystem.Helpers;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryApiController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryApiController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet("recent")]
        public async Task<IActionResult> Recent([FromQuery] int take = 20)
        {
            var items = await _inventoryService.GetRecentAsync(take);
            return Ok(items);
        }

        [HttpGet("history")]
        public async Task<IActionResult> History([FromQuery] int page = 1, [FromQuery] string? type = null)
        {
            var result = await _inventoryService.GetPagedHistoryAsync(page, 20, type);
            return Ok(result);
        }

        [HttpPost("receive")]
        public async Task<IActionResult> Receive([FromBody] StockRequest request)
        {
            var userId = HttpContext.Session.GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized();

            var (success, error) = await _inventoryService.RecordTransactionAsync(
                request.ProductId, "IN", request.Quantity, userId.Value, request.Notes ?? "", request.Shift ?? "Morning");

            return success ? Ok(new { message = "Stock received" }) : BadRequest(new { error });
        }

        [HttpPost("dispense")]
        public async Task<IActionResult> Dispense([FromBody] StockRequest request)
        {
            var userId = HttpContext.Session.GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized();

            var (success, error) = await _inventoryService.RecordTransactionAsync(
                request.ProductId, "OUT", request.Quantity, userId.Value, request.Notes ?? "", request.Shift ?? "Morning");

            return success ? Ok(new { message = "Stock dispensed" }) : BadRequest(new { error });
        }

        public class StockRequest
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public string? Notes { get; set; }
            public string? Shift { get; set; }
        }
    }
}
