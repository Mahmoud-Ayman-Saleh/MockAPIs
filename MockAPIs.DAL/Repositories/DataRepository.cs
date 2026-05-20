using Microsoft.EntityFrameworkCore;
using MockAPIs.DAL.Data;
using MockAPIs.DAL.Models;
using MockAPIs.DAL.Repositories.Interfaces;

namespace MockAPIs.DAL.Repositories
{
    public class DataRepository : IDataRepository
    {
        private readonly ApplicationDbContext _context;

        public DataRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // loads resource WITH its fields
        // fields are needed so Bogus knows what to generate
        public async Task<Resource?> GetResourceWithFields(Guid resourceId)
        {
            return await _context.Resources
                .Include(r => r.Fields)
                .FirstOrDefaultAsync(r => r.Id == resourceId);
        }

        // one EXISTS query with one JOIN — no entities loaded
        public async Task<bool> IsResourceOwnedByUser(Guid resourceId, Guid userId)
        {
            return await _context.Resources
                .AnyAsync(r => r.Id == resourceId
                            && r.Project.UserId == userId);
        }

        // delete all existing records before regenerating
        // so we don't stack duplicate data on top of old data
        public async Task DeleteExistingRecords(Guid resourceId)
        {
            var records = _context.MockRecords
                .Where(m => m.ResourceId == resourceId);

            _context.MockRecords.RemoveRange(records);
        }

        public async Task AddRecords(List<MockRecord> records)
        {
            await _context.MockRecords.AddRangeAsync(records);
        }

        // keep Resource.Count in sync with how many records were generated
        public async Task UpdateResourceCount(Guid resourceId, int count)
        {
            var resource = await _context.Resources
                .FirstOrDefaultAsync(r => r.Id == resourceId);

            if (resource != null)
                resource.Count = count;
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}