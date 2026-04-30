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
    public class ResourceRepository : IResourceRepository
    {
        private readonly ApplicationDbContext context;

        public ResourceRepository(ApplicationDbContext _context)
        {
            context = _context;
        }
         public async Task<bool> IsSlugUniqueInProjectAsync(Guid projectId, string slug)
        {
            return !await context.Resources.AnyAsync(r => r.ProjectId == projectId && r.Slug == slug);
        }

    }
}