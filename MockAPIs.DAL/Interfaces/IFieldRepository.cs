using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockAPIs.DAL.Interfaces
{
    public interface IFieldRepository
    {
        Task<bool> IsResourceOwnedByUserAsync(Guid resourceId, Guid userId);
        Task<bool> IsFieldOwnedByUserAsync(Guid fieldId, Guid userId);
    }
}