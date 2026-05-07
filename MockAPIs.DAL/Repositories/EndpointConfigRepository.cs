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
    public class EndpointConfigRepository : IEndpointConfigRepository
    {
        private readonly ApplicationDbContext context;

        public EndpointConfigRepository(ApplicationDbContext _context)
        {
            context = _context;
        }
        public async Task<EndpointConfig?> GetByResourceId(Guid resourceId)
        {
            return await context.EndpointConfigs.FirstOrDefaultAsync(e => e.ResourceId == resourceId);
        }

        public async Task<bool> IsResourceOwnedByUser(Guid resourceId, Guid userId)
        {
            return await context.Resources.AnyAsync(r => r.Id == resourceId && r.Project.UserId == userId);
        }
    }
}