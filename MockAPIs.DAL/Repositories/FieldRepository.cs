using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.DAL.Interfaces;

namespace MockAPIs.DAL.Repositories
{
    public class FieldRepository : IFieldRepository
    {
        public Task<bool> IsFieldOwnedByUserAsync(Guid fieldId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsResourceOwnedByUserAsync(Guid resourceId, Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}