using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.BLL.DTOs;
using MockAPIs.BLL.Interfaces;
using MockAPIs.DAL.Enums;
using MockAPIs.DAL.Interfaces;
using MockAPIs.DAL.Models;

namespace MockAPIs.BLL.Services
{
    public class FieldService : IFieldService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IFieldRepository fieldRepository;

        public FieldService(IUnitOfWork _unitOfWork, IFieldRepository _fieldRepository)
        {
            unitOfWork = _unitOfWork;
            fieldRepository = _fieldRepository;
        }   
        public async Task<FieldCreatedDto> Create(Guid resourceId, CreateFieldDto dto, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException("Field name is required");

            if (!Enum.TryParse<FieldDataType>(dto.DataType, ignoreCase: true, out var parsedDataType))
                throw new ValidationException($"Invalid data type '{dto.DataType}'");

            // verify the resource belongs to this user
            // one EXISTS query with one JOIN — no entities loaded
            var resourceOwned = await fieldRepository.IsResourceOwnedByUser(resourceId, userId);

            if (!resourceOwned)
                throw new InvalidOperationException("Resource not found");

            var field = new Field
            {
                Id = Guid.NewGuid(),
                ResourceId = resourceId,
                Name = dto.Name,
                DataType = parsedDataType,
                FakerHint = dto.FakerHint,
                IsRequired = dto.IsRequired
            };

            await unitOfWork.Fields.Add(field);
            await unitOfWork.Fields.SaveChanges();

            return FieldCreatedDto.FromEntity(field);
        }

        public async Task Delete(Guid resourceId, Guid fieldId, Guid userId)
        {
            var fieldOwned = await fieldRepository.IsFieldOwnedByUser(fieldId, userId);

            if (!fieldOwned)
                throw new KeyNotFoundException("Field not found");

            // now load only the field entity itself to delete it
            var field = await unitOfWork.Fields.GetById(fieldId);

            // double check the field belongs to the given resourceId in the URL
            if (field!.ResourceId != resourceId)
                throw new KeyNotFoundException("Field not found");

            await unitOfWork.Fields.Delete(field);
            await unitOfWork.Fields.SaveChanges();
        }

        public async Task<List<FieldDto>> GetAll(Guid resourceId, Guid userId)
        {
            var resourceOwned = await fieldRepository.IsResourceOwnedByUser(resourceId, userId);

            if (!resourceOwned) throw new KeyNotFoundException("Resource not found");

            List<Field> fields = await fieldRepository.GetAllByResourceId(resourceId);

            int n = fields.Count;

            List<FieldDto> ans = new List<FieldDto>(n);

            for (int i = 0; i < n; i++)
            {
                ans[i] = FieldDto.FromEntity(fields[i]);
            }
            
            return ans;
        }



        public async Task<FieldCreatedDto> Update(Guid resourceId, Guid fieldId, UpdateFieldDto dto, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException("Field name is required");

            if (!Enum.TryParse<FieldDataType>(dto.DataType, ignoreCase: true, out var parsedDataType))
                throw new ValidationException($"Invalid data type '{dto.DataType}'");

            // verify the field belongs to this user
            // one EXISTS query with two JOINs (Field → Resource → Project) — no entities loaded
            var fieldOwned = await fieldRepository.IsFieldOwnedByUser(fieldId, userId);

            if (!fieldOwned)
                throw new KeyNotFoundException("Field not found");

            // now load only the field entity itself to update it
            var field = await unitOfWork.Fields.GetById(fieldId);

            // double check the field actually belongs to the given resourceId in the URL
            if (field!.ResourceId != resourceId)
                throw new KeyNotFoundException("Field not found");

            field.Name = dto.Name;
            field.DataType = parsedDataType;
            field.FakerHint = dto.FakerHint;
            field.IsRequired = dto.IsRequired;

            await unitOfWork.Fields.Update(field);
            await unitOfWork.Fields.SaveChanges();

            return FieldCreatedDto.FromEntity(field);
        }
    }
}