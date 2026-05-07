using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.BLL.DTOs;
using MockAPIs.BLL.Interfaces;

namespace MockAPIs.BLL.Services
{
    public class EndpointConfigService : IEndpointConfigService
    {
        public Task<EndpointConfigResponseDto> UpdateEndpointConfigAsync(Guid resourceId, UpdateEndpointConfigDto dto, Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}