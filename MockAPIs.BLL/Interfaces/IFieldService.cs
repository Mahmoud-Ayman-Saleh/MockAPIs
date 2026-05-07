using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.BLL.DTOs;

namespace MockAPIs.BLL.Interfaces
{
    public interface IFieldService
    {
        Task<List<FieldDto>> GetAll(Guid resourceId, Guid userId);
        Task<FieldCreatedDto> Create(Guid resourceId, CreateFieldDto dto, Guid userId);
        Task<FieldCreatedDto> Update(Guid resourceId, Guid fieldId, UpdateFieldDto dto, Guid userId);
        Task Delete(Guid resourceId, Guid fieldId, Guid userId);
    }
}