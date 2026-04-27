using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MockAPIs.DAL.Data;
using MockAPIs.DAL.Interfaces;
using MockAPIs.DAL.Models;

namespace MockAPIs.DAL.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext context;

        public ProjectRepository(ApplicationDbContext _context)
        {
            context = _context;
        }


        public async Task<List<Project>> GetAllByUserId(Guid userId)
        {
            return await context.Projects.Where(p => p.UserId == userId).ToListAsync();
        }

        public async Task<Project?> GetById(Guid id)
        {
            return await context.Projects.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Project?> GetByIdWithResources(Guid id)
        {
            return await context.Projects.Include(p => p.Resources).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> IsSlugUnique(string slug)
        {
            return !await context.Projects.AnyAsync(p => p.Slug == slug);
        }

        public async Task<bool> IsTokenUnique(string token)
        {
            return !await context.Projects.AnyAsync(p => p.Token == token);
        }
    }
}