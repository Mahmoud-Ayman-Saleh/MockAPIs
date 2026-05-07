using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.DAL.Interfaces;
using MockAPIs.DAL.Models;

namespace MockAPIs.DAL.Repositories
{
    public class EndpointConfigRepository : IEndpointConfigRepository
    {
        public Task<EndpointConfig?> GetByResourceIdAsync(Guid resourceId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsResourceOwnedByUserAsync(Guid resourceId, Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}