using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockAPIs.BLL.DTOs;
using MockAPIs.BLL.Interfaces;

namespace MockAPIs.API.Controllers
{
    [ApiController]
    [Route("api/resources/{resourceId:guid}/fields")]
    [Authorize]
    public class FieldController : ControllerBase
    {
        private readonly IFieldService fieldService;
        public FieldController(IFieldService _fieldService)
        {
            fieldService = _fieldService;
        }

        // GET /api/resources/{resourceId}/fields
        [HttpGet]
        public async Task<IActionResult> GetAll(Guid resourceId)
        {
            var userId = GetCurrentUserId();
            var fields = await fieldService.GetAll(resourceId, userId);
            return Ok(fields);
        }

        // POST /api/resources/{resourceId}/fields
        [HttpPost]
        public async Task<IActionResult> Create(Guid resourceId, [FromBody] CreateFieldDto dto)
        {
            var userId = GetCurrentUserId();
            var field = await fieldService.Create(resourceId, dto, userId);
            return StatusCode(201, field);
        }

        // PUT /api/resources/{resourceId}/fields/{fieldId}
        [HttpPut("{fieldId:guid}")]
        public async Task<IActionResult> Update(Guid resourceId, Guid fieldId, [FromBody] UpdateFieldDto dto)
        {
            var userId = GetCurrentUserId();
            var field = await fieldService.Update(resourceId, fieldId, dto, userId);
            return Ok(field);
        }

        // DELETE /api/resources/{resourceId}/fields/{fieldId}
        [HttpDelete("{fieldId:guid}")]
        public async Task<IActionResult> Delete(Guid resourceId, Guid fieldId)
        {
            var userId = GetCurrentUserId();
            await fieldService.Delete(resourceId, fieldId, userId);
            return Ok(new { message = "Field deleted successfully" });
        }

        // ── Helper ─────────────────────────────────────────────
        private Guid GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdStr!);
        }

    }
}