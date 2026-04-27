using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockAPIs.BLL.Interfaces;

namespace MockAPIs.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectServices projectServices;

        public ProjectController(IProjectServices _projectServices)
        {
            projectServices = _projectServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetCurrentUserId();
            var projects = await projectServices.GetAll(userId);
            return Ok(projects);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid projectId)
        {
            var userId = GetCurrentUserId();
            var project = await projectServices.GetById(projectId, userId);

            if (project == null) return NotFound(new { message = "Project not found" });

            return Ok(project);
        }

        [HttpPut("{id:guid}/rename")]
        public async Task<IActionResult> Rename(Guid id, [FromBody] string newName)
        {
            var userId = GetCurrentUserId();
            var project = await projectServices.Rename(id, newName, userId);

            if (project == null)
                return NotFound(new { message = "Project not found" });

            return Ok(project);
        }

         [HttpPost]
        public async Task<IActionResult> Create([FromBody] string name)
        {
            var userId = GetCurrentUserId();
            var project = await projectServices.Create(name, userId);
            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
        }


        // DELETE /api/projects/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetCurrentUserId();
            var deleted = await projectServices.Delete(id, userId);

            if (!deleted)
                return NotFound(new { message = "Project not found" });

            return Ok(new { message = "Project deleted successfully" });
        }
        // Helper function
        private Guid GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdStr!);
        }
    }
}