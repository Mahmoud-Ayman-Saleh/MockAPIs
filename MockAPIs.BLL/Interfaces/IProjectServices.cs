using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.BLL.DTOs;
using MockAPIs.DAL.Models;

namespace MockAPIs.BLL.Interfaces
{
    public interface IProjectServices
    {
        Task<List<ProjectSummaryDto>> GetAll(Guid userId);
        Task<ProjectDetailsDto> GetById(Guid projectId, Guid userId);
        Task<ProjectRenamedDto> Rename(Guid projectId, string newName, Guid userId);
        Task<ProjectCreatedDto> Create(string name, Guid userId);
        Task<bool> Delete(Guid projectId, Guid userId);
    }
}