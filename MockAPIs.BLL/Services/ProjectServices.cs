using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.BLL.DTOs;
using MockAPIs.BLL.Interfaces;
using MockAPIs.DAL.Interfaces;
using MockAPIs.DAL.Models;

namespace MockAPIs.BLL.Services
{
    public class ProjectServices : IProjectServices
    {
        private readonly IProjectRepository repository;
        private readonly IUnitOfWork unitOfWork;

        public ProjectServices(IProjectRepository _repository, IUnitOfWork _unitOfWork)
        {
            repository = _repository;
            unitOfWork = _unitOfWork;
        }
        public async Task<ProjectCreatedDto> Create(string name, Guid userId)
        {
            var slug = GenerateSlug(name);
            var token = await GenerateUniqueToken();
            var finalSlug = await EnsureUniqueSlug(slug);

            var project = new Project
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = name,
                Slug = finalSlug,
                Token = token,
                BaseUrl = $"https://mockapis.io/{token}/api/v1",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            await unitOfWork.Projects.Add(project);
            await unitOfWork.Projects.SaveChanges();

            return ProjectCreatedDto.FromEntity(project);

        }


        public async Task<bool> Delete(Guid projectId, Guid userId)
        {
            var project = await repository.GetById(projectId);

            if (project == null || project.UserId != userId) return false;

            await unitOfWork.Projects.Delete(project);
            await unitOfWork.Projects.SaveChanges();

            return true;

        }

        public async Task<List<ProjectSummaryDto>> GetAll(Guid userId)
        {
            var projects = await repository.GetAllByUserId(userId);

            List<ProjectSummaryDto> ans = new List<ProjectSummaryDto>();

            for (int i = 0; i < projects.Count(); i++)
            {
                ans.Add(ProjectSummaryDto.FromEntity(projects[i]));
            }
            return ans;
        }

        public async Task<ProjectDetailsDto> GetById(Guid projectId, Guid userId)
        {
            var project = await repository.GetByIdWithResources(projectId);

            if (project == null || project.UserId != userId) return null;

            return ProjectDetailsDto.FromEntity(project);
        }

        public async Task<ProjectRenamedDto> Rename(Guid projectId, string newName, Guid userId)
        {
            var project = await repository.GetById(projectId);

            if (project == null || project.UserId != userId) return null;

            var newSlug = GenerateSlug(newName);
            var finalSlug = await EnsureUniqueSlug(newSlug);

            project.Name = newName;
            project.Slug = finalSlug;

            await unitOfWork.Projects.Update(project);
            await unitOfWork.Projects.SaveChanges();

            return ProjectRenamedDto.FromEntity(project); 
        }


        // Helper functions 
        private string GenerateSlug(string name)
        {
            return name.ToLower().Trim().Replace(" ", "-").Replace("_", "-");
        }

        private async Task<string> GenerateUniqueToken()
        {
            string token;
            do
            {
                token = Convert.ToHexString(Guid.NewGuid().ToByteArray()).ToLower().Substring(0,24);
            } while (!await repository.IsTokenUnique(token));
            return token;
        }
        private async Task<string> EnsureUniqueSlug(string slug)
        {
            var finalSlug = slug;
            var counter = 1;
            while (!await repository.IsSlugUnique(finalSlug))
            {
                finalSlug = $"{slug}-{counter}";
                counter++;
            }

            return finalSlug;
        }
    }
}