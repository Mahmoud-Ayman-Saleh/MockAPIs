using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.DAL.Models;

namespace MockAPIs.DAL.Interfaces
{
    public interface IEndpointConfigRepository
    {
        Task<EndpointConfig?> GetByResourceIdAsync(Guid resourceId);
        Task<bool> IsResourceOwnedByUserAsync(Guid resourceId, Guid userId);
    }
}