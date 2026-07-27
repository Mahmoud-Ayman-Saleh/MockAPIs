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
    public class FieldRepository : IFieldRepository
    {
        private readonly ApplicationDbContext context;

        public FieldRepository(ApplicationDbContext _context)
        {
            context = _context;
        }
        public async Task<List<Field>> GetAllByResourceId(Guid resourceId)
        {
            return await context.Fields.Where(f => f.ResourceId == resourceId).ToListAsync();
        }

        public async Task<bool> IsFieldOwnedByUser(Guid fieldId, Guid userId)
        {
            return await context.Fields.AnyAsync(f => f.Id == fieldId && f.Resource.Project.UserId == userId);
        }

        public async Task<bool> IsResourceOwnedByUser(Guid resourceId, Guid userId)
        {
            return await context.Resources.AnyAsync(r => r.Id == resourceId && r.Project.UserId == userId);
        }

        public async Task<bool> IsFieldOwnedByUserAsync(Guid fieldId, Guid userId)
        {
            return await context.Fields.AnyAsync(f => f.Id == fieldId && f.Resource.Project.UserId == userId);
        }
    }
}