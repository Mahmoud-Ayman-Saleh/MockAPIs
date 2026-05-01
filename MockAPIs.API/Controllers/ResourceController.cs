using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MockAPIs.BLL.Interfaces;

namespace MockAPIs.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResourceController : ControllerBase
    {
        private readonly IResourceServices resourceService;
        public ResourceController(IResourceServices _resourceService)
        {
            resourceService = _resourceService;
        }

        [HttpPost("{id:guid}")]
        public async Task<IActionResult> Create(Guid id, [FromBody]string name)
        {
            var userId = GetCurrentUserId();
            var resource = await resourceService.Create(name, id, userId);
            return StatusCode(201, resource);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetCurrentUserId();
            var deleted = await resourceService.Delete(id, userId);
            if (!deleted)
                return NotFound(new { message = "Resource not found" });

            return Ok(new { message = "Resource deleted successfully" });
        }


        private Guid GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdStr!);
        }
        
    }
}