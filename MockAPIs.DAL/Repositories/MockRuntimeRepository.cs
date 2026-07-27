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
    public class MockRuntimeRepository : IMockRuntimeRepository
    {
        private ApplicationDbContext context;
        public MockRuntimeRepository(ApplicationDbContext _context)
        {
            context = _context;
        }

        public async Task AddRecord(MockRecord record)
        {
            await context.MockRecords.AddAsync(record);
        }

        public async Task DeleteRecord(MockRecord record)
        {
            context.MockRecords.Remove(record);
        }

        public async Task<List<MockRecord>> GetAllRecords(Guid resourceId)
        {
            return await context.MockRecords.Where(m => m.ResourceId == resourceId).ToListAsync();
        }

        public async Task<(List<MockRecord> Records, int TotalCount)> GetPagedRecordsAsync(Guid resourceId, int page, int pageSize)
        {
            var query = context.MockRecords.Where(m => m.ResourceId == resourceId).AsNoTracking();
            var totalCount = await query.CountAsync();
            var records = await query.OrderBy(m => m.CreatedAt)
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();
            return (records, totalCount);
        }

        public async Task<Project?> GetProjectByToken(string token)
        {
            return await context.Projects.FirstOrDefaultAsync(p => p.Token == token && p.IsActive);
        }

        public async Task<MockRecord?> GetRecordById(Guid resourceId, Guid recordId)
        {
            return await context.MockRecords.FirstOrDefaultAsync(m => m.ResourceId == resourceId && m.Id == recordId);
        }

        public async Task<Resource?> GetResourceWithConfig(Guid projectId, string resourceSlug)
        {
            return await context.Resources
                .Include(r => r.EndpointConfig)
                .FirstOrDefaultAsync(r => r.ProjectId == projectId
                                       && r.Slug == resourceSlug);

        }

        public async Task SaveChanges()
        {
            await context.SaveChangesAsync();
        }

        public async Task UpdateRecord(MockRecord record)
        {
            context.MockRecords.Update(record);
        }
    }
}