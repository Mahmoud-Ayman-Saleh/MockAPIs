using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MockAPIs.DAL.Data;
using MockAPIs.DAL.Interfaces;

namespace MockAPIs.DAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext context;
        private readonly DbSet<T> dbSet;

        public GenericRepository(ApplicationDbContext _context)
        {
            context = _context;
            dbSet = _context.Set<T>();
        }
        public async Task<T> Add(T entity)
        {
            await dbSet.AddAsync(entity);
            return entity;
        }

        public async Task Delete(T entity)
        {
            if (entity != null) dbSet.Remove(entity);
        }

        public async Task<bool> Exist(Guid id)
        {
            return await dbSet.FindAsync(id) != null;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<T?> GetById(Guid id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task SaveChanges()
        {
            await context.SaveChangesAsync();
        }

        public Task Update(T entity)
        {
            dbSet.Update(entity);
            return Task.CompletedTask;
        }
    }
}