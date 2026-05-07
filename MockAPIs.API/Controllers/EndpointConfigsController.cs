using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MockAPIs.API.Controllers
{
    [ApiController]
    [Route("api/resources/{resourceId:guid}/endpoint-config")]
    [Authorize]
    public class EndpointConfigsController : ControllerBase
    {
        
    }
}