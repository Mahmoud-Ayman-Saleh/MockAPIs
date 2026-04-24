using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MockAPIs.DAL.Interfaces;

namespace MockAPIs.DAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbContext context;
        private readonly DbSet<T> dbSet;

        public GenericRepository(DbContext _context)
        {
            context = _context;
            dbSet = _context.Set<T>();
        }
        public async Task<T> Add(T entity)
        {
            await dbSet.AddAsync(entity);
            return entity;
        }

        public async Task Delete(int id)
        {
            var entity = await dbSet.FindAsync(id);

            if (entity != null) dbSet.Remove(entity);
        }

        public async Task<bool> Exist(int id)
        {
            return await dbSet.FindAsync(id) != null;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<T?> GetById(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public Task Update(T entity)
        {
            dbSet.Update(entity);
            return Task.CompletedTask;
        }
    }
}