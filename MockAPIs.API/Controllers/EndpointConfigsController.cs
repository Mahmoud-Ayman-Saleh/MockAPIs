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
    [Route("api/resources/{resourceId:guid}/endpoint-config")]
    [Authorize]
    public class EndpointConfigsController : ControllerBase
    {
        private readonly IEndpointConfigService _endpointConfigService;

        public EndpointConfigsController(IEndpointConfigService endpointConfigService)
        {
            _endpointConfigService = endpointConfigService;
        }

        // PUT /api/resources/{resourceId}/endpoint-config
        [HttpPut]
        public async Task<IActionResult> Update(
            Guid resourceId,
            [FromBody] UpdateEndpointConfigDto dto)
        {
            var userId = GetCurrentUserId();

            var config = await _endpointConfigService
                .Update(resourceId, dto, userId);

            return Ok(config);
        }

        // helper func
        private Guid GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdStr!);
        }
    }
}