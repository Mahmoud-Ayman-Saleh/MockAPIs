using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MockAPIs.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResourceController : ControllerBase
    {
        private readonly IResourceService resourceService;
        public ResourceController(IResourceService _resourceService)
        {
            resourceService = _resourceService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string name, [FromBody] Guid projectId)
        {
            var resource = resourceService.Create(name, projectId);
            return CreatedAtAction();
        }
        
    }
}