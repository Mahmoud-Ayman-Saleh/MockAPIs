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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Guid projectId, string name)
        {
            var userId = GetCurrentUserId();
            var resource = await resourceService.Create(name, projectId, userId);
            return StatusCode(201, resource);
        }


        private Guid GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdStr!);
        }
        
    }
}