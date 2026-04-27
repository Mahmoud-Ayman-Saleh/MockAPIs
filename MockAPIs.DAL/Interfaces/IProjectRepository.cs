using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.DAL.Models;

namespace MockAPIs.DAL.Interfaces
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetAllByUserId(Guid userId);
        Task<Project?> GetById(Guid id);
        Task<Project?> GetByIdWithResources(Guid id);
        Task<bool> IsTokenUnique(string token);
        Task<bool> IsSlugUnique(string slug);
    }
}