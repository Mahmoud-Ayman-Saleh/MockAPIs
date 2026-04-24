using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.DAL.Models;

namespace MockAPIs.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<AppUser> Users {get;}
        IGenericRepository<Project> Projects {get;}
        IGenericRepository<Field> Fields {get;}
        IGenericRepository<Resource> Resources {get;}
        IGenericRepository<MockRecord> MockRecords {get;}
        IGenericRepository<EndpointConfig> EndpointsConfig {get;}

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

    }
}