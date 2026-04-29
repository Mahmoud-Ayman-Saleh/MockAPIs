using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.BLL.DTOs;

namespace MockAPIs.BLL.Interfaces
{
    public interface IResourceServices
    {
        Task<ResourceCreated> Created(string name, Guid projectId);
        Task Delete(Guid resourceId, Guid projectId);
    }
}