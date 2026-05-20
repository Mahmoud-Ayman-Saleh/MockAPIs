using MockAPIs.BLL.DTOs;

namespace MockAPIs.BLL.Interfaces
{
    public interface IDataService
    {
        Task<PreviewResponseDto> GetPreview(Guid resourceId, Guid userId);
        Task<GenerateResponseDto> GenerateData(Guid resourceId, GenerateRequestDto dto, Guid userId);
    }
}