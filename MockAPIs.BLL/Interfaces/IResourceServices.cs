using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.BLL.DTOs;

namespace MockAPIs.BLL.Interfaces
{
    public interface IResourceServices
    {
        Task<ResourceCreatedDto> Create(string name, Guid projectId, Guid userId);
        Task<bool> Delete(Guid resourceId, Guid userId);
    }
}