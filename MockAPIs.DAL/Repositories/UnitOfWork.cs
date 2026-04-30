using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MockAPIs.DAL.Data;
using MockAPIs.DAL.Interfaces;
using MockAPIs.DAL.Models;

namespace MockAPIs.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;
        private IDbContextTransaction? transaction;
        private IGenericRepository<AppUser> users;
        private IGenericRepository<Resource> resources;
        private IGenericRepository<Field> fields;
        private IGenericRepository<Project> projects;
        private IGenericRepository<MockRecord> mockRecords;
        private IGenericRepository<EndpointConfig> endpointsConfig;

        public UnitOfWork(ApplicationDbContext _context)
        {
            context = _context;
        }

        public IGenericRepository<AppUser> Users
        {
            get
            {
                if (users == null)
                {
                    users = new GenericRepository<AppUser>(context);
                }
                return users;
            }
        }

        public IGenericRepository<Project> Projects
        {
            get
            {
                if (projects == null)
                {
                    projects = new GenericRepository<Project>(context);
                }
                return projects;
            }
        }

        public IGenericRepository<Resource> Resources
        {
            get
            {
                if (resources == null)
                {
                    resources = new GenericRepository<Resource>(context);
                }
                return resources;
            }
        }

        public IGenericRepository<Field> Fields
        {
            get
            {
                if (fields == null)
                {
                    fields = new GenericRepository<Field>(context);
                }
                return fields;
            }
        }

        public IGenericRepository<MockRecord> MockRecords
        {
            get
            {
                if (mockRecords == null)
                {
                    mockRecords = new GenericRepository<MockRecord>(context);
                }
                return mockRecords;
            }
        }

        public IGenericRepository<EndpointConfig> EndpointsConfig
        {
            get
            {
                if (endpointsConfig == null)
                {
                    endpointsConfig = new GenericRepository<EndpointConfig>(context);
                }
                return endpointsConfig;
            }
        }

        public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();

        public async Task BeginTransactionAsync()
        {
            transaction = await context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (transaction != null)
            {
                await transaction.CommitAsync();
                await transaction.DisposeAsync();
                transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (transaction != null)
            {
                await transaction.RollbackAsync();
                await transaction.DisposeAsync();
                transaction = null;
            }
        }

        public void Dispose()
        {
            transaction?.Dispose();
            context.Dispose();
        }
    }
}
