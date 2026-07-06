using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockAPIs.BLL.Interfaces
{
    public interface IMockRuntimeService
    {
        Task<object> GetListAsync(string token, string resourceSlug, string? search, int? page, int? limit);
        Task<Dictionary<string, object?>> GetByIdAsync(string token, string resourceSlug, Guid recordId);
        Task<Dictionary<string, object?>> CreateRecordAsync(string token, string resourceSlug, Dictionary<string, object?> body);
        Task<Dictionary<string, object?>> UpdateRecordAsync(string token, string resourceSlug, Guid recordId, Dictionary<string, object?> body);
        Task DeleteRecordAsync(string token, string resourceSlug, Guid recordId);
    }
}