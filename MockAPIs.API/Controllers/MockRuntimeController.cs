using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MockAPIs.BLL.Interfaces;

namespace MockAPIs.API.Controllers
{
    [ApiController]
    [Route("{token}/api/v1/{resourceSlug}")]
    public class MockRuntimeController : ControllerBase
    {
        private readonly IMockRuntimeService _mockRuntimeService;

        public MockRuntimeController(IMockRuntimeService mockRuntimeService)
        {
            _mockRuntimeService = mockRuntimeService;
        }

        // GET /{token}/api/v1/{resourceSlug}
        // GET /{token}/api/v1/{resourceSlug}?page=1&limit=10
        // GET /{token}/api/v1/{resourceSlug}?search=chair
        [HttpGet]
        public async Task<IActionResult> GetList(
            string token,
            string resourceSlug,
            [FromQuery] string? search,
            [FromQuery] int? page,
            [FromQuery] int? limit)
        {
            var result = await _mockRuntimeService
                .GetListAsync(token, resourceSlug, search, page, limit);

            return Ok(result);
        }

        // GET /{token}/api/v1/{resourceSlug}/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            string token,
            string resourceSlug,
            Guid id)
        {
            var record = await _mockRuntimeService
                .GetByIdAsync(token, resourceSlug, id);

            return Ok(record);
        }

        // POST /{token}/api/v1/{resourceSlug}
        [HttpPost]
        public async Task<IActionResult> Create(
            string token,
            string resourceSlug,
            [FromBody] Dictionary<string, JsonElement> body)
        {
            // convert JsonElement values to plain objects before passing to service
            var converted = body.ToDictionary(
                kvp => kvp.Key,
                kvp => ConvertJsonElement(kvp.Value)
            );

            var record = await _mockRuntimeService
                .CreateRecordAsync(token, resourceSlug, converted);

            return StatusCode(201, record);
        }

        // PUT /{token}/api/v1/{resourceSlug}/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            string token,
            string resourceSlug,
            Guid id,
            [FromBody] Dictionary<string, JsonElement> body)
        {
            var converted = body.ToDictionary(
                kvp => kvp.Key,
                kvp => ConvertJsonElement(kvp.Value)
            );

            var record = await _mockRuntimeService
                .UpdateRecordAsync(token, resourceSlug, id, converted);

            return Ok(record);
        }

        // DELETE /{token}/api/v1/{resourceSlug}/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            string token,
            string resourceSlug,
            Guid id)
        {
            await _mockRuntimeService.DeleteRecordAsync(token, resourceSlug, id);
            return Ok(new { message = "Record deleted successfully" });
        }

        // ── Helper
        private object? ConvertJsonElement(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.TryGetInt64(out var l) ? l : element.GetDouble(),
                JsonValueKind.True   => true,
                JsonValueKind.False  => false,
                JsonValueKind.Null   => null,
                _                    => element.ToString()
            };
        }
    }
}