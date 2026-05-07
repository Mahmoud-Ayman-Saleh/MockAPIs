using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.BLL.DTOs;

namespace MockAPIs.BLL.Interfaces
{
    public interface IEndpointConfigService
    {
        Task<EndpointConfigResponseDto> UpdateEndpointConfigAsync(Guid resourceId,
            UpdateEndpointConfigDto dto, Guid userId);
    }
}