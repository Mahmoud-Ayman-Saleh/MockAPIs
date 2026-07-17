using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.DAL.Models;

namespace MockAPIs.DAL.Interfaces
{
    public interface IMockRuntimeRepository
    {
        Task<Project?> GetProjectByToken(string token);
        Task<Resource?> GetResourceWithConfig(Guid projectId, string resourceSlug);
        Task<List<MockRecord>> GetAllRecords(Guid resourceId);
        Task<(List<MockRecord> Records, int TotalCount)> GetPagedRecordsAsync(Guid resourceId, int page, int pageSize);
        Task<MockRecord?> GetRecordById(Guid resourceId, Guid recordId);
        Task AddRecord(MockRecord record);
        Task UpdateRecord(MockRecord record);
        Task DeleteRecord(MockRecord record);
        Task SaveChanges();
    }
}