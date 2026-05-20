// MockAPIs.BLL/Services/DataService.cs
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using MockAPIs.BLL.DTOs;
using MockAPIs.BLL.Helpers;
using MockAPIs.BLL.Interfaces;
using MockAPIs.DAL.Models;
using MockAPIs.DAL.Repositories.Interfaces;

namespace MockAPIs.BLL.Services
{
    public class DataService : IDataService
    {
        private readonly IDataRepository _dataRepository;

        public DataService(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public async Task<PreviewResponseDto> GetPreview(Guid resourceId, Guid userId)
        {
            // verify ownership
            var owned = await _dataRepository.IsResourceOwnedByUser(resourceId, userId);

            if (!owned)
                throw new KeyNotFoundException("Resource not found");

            // load resource with its fields
            var resource = await _dataRepository.GetResourceWithFields(resourceId);

            if (resource == null)
                throw new KeyNotFoundException("Resource not found");

            if (resource.Fields == null || !resource.Fields.Any())
                throw new ValidationException("Resource has no fields defined yet");

            // generate 3 preview records — never saved to DB
            var previewData = FakerEngine.Generate(resource.Fields.ToList(), 3);

            return new PreviewResponseDto
            {
                ResourceId = resourceId,
                Preview = previewData
            };
        }

        public async Task<GenerateResponseDto> GenerateData(
            Guid resourceId,
            GenerateRequestDto dto,
            Guid userId)
        {
            // validate count
            if (dto.Count <= 0)
                throw new ValidationException("Count must be greater than 0");

            if (dto.Count > 1000)
                throw new ValidationException("Count cannot exceed 1000 records");

            // verify ownership
            var owned = await _dataRepository.IsResourceOwnedByUser(resourceId, userId);

            if (!owned)
                throw new KeyNotFoundException("Resource not found");

            // load resource with its fields
            var resource = await _dataRepository.GetResourceWithFields(resourceId);

            if (resource == null)
                throw new KeyNotFoundException("Resource not found");

            if (resource.Fields == null || !resource.Fields.Any())
                throw new ValidationException("Resource has no fields defined yet");

            // generate the records using Bogus
            var generatedData = FakerEngine.Generate(resource.Fields.ToList(), dto.Count);

            // convert each dictionary to a JSON string for storage in JSONB column
            var mockRecords = generatedData.Select(record => new MockRecord
            {
                Id = Guid.NewGuid(),
                ResourceId = resourceId,
                Data = JsonSerializer.Serialize(record),
                CreatedAt = DateTime.UtcNow
            }).ToList();

            // delete old records first then insert new ones
            await _dataRepository.DeleteExistingRecords(resourceId);
            await _dataRepository.AddRecords(mockRecords);

            // update Resource.Count to reflect new count
            await _dataRepository.UpdateResourceCount(resourceId, dto.Count);

            await _dataRepository.SaveChanges();

            return new GenerateResponseDto
            {
                ResourceId = resourceId,
                GeneratedCount = dto.Count,
                Message = "Mock data generated successfully"
            };
        }
    }
}