using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.DAL.Models;

namespace MockAPIs.DAL.Interfaces
{
    public interface IFieldRepository
    {
        Task<bool> IsResourceOwnedByUser(Guid resourceId, Guid userId);
        Task<bool> IsFieldOwnedByUser(Guid fieldId, Guid userId);
        Task<List<Field>> GetAllByResourceId(Guid resourceId);
    }
}