using MockAPIs.DAL.Models;

namespace MockAPIs.DAL.Repositories.Interfaces
{
    public interface IDataRepository
    {
        Task<Resource?> GetResourceWithFields(Guid resourceId);
        Task<bool> IsResourceOwnedByUser(Guid resourceId, Guid userId);
        Task DeleteExistingRecords(Guid resourceId);
        Task AddRecords(List<MockRecord> records);
        Task UpdateResourceCount(Guid resourceId, int count);
        Task SaveChanges();
    }
}