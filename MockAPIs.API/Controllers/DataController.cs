// MockAPIs.API/Controllers/DataController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockAPIs.BLL.DTOs;
using MockAPIs.BLL.Interfaces;
using System.Security.Claims;

namespace MockAPIs.API.Controllers
{
    [ApiController]
    [Route("api/resources/{resourceId:guid}")]
    [Authorize]
    public class DataController : ControllerBase
    {
        private readonly IDataService _dataService;

        public DataController(IDataService dataService)
        {
            _dataService = dataService;
        }

        // GET /api/resources/{resourceId}/preview
        [HttpGet("preview")]
        public async Task<IActionResult> Preview(Guid resourceId)
        {
            var userId = GetCurrentUserId();
            var preview = await _dataService.GetPreview(resourceId, userId);
            return Ok(preview);
        }

        // POST /api/resources/{resourceId}/generate
        [HttpPost("generate")]
        public async Task<IActionResult> Generate(
            Guid resourceId,
            [FromBody] GenerateRequestDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _dataService.GenerateData(resourceId, dto, userId);
            return Ok(result);
        }

        // ── Helper ─────────────────────────────────────────────
        private Guid GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdStr!);
        }
    }
}