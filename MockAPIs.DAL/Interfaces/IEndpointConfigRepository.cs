using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.DAL.Models;

namespace MockAPIs.DAL.Interfaces
{
    public interface IEndpointConfigRepository
    {
        Task<EndpointConfig?> GetByResourceId(Guid resourceId);
        Task<bool> IsResourceOwnedByUser(Guid resourceId, Guid userId);
    }
}