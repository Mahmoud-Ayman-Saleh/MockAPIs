using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockAPIs.DAL.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetById(Guid id);
        Task<IEnumerable<T>> GetAll();
        Task Update(T entity);
        Task<T> Add(T entity);
        Task Delete(T entity);
        Task<bool> Exist(Guid id);
        Task SaveChanges();
    }
}