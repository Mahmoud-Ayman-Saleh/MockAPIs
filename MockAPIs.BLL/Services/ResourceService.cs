using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using MockAPIs.BLL.DTOs;
using MockAPIs.BLL.Interfaces;
using MockAPIs.DAL.Interfaces;
using MockAPIs.DAL.Models;

namespace MockAPIs.BLL.Services
{
    public class ResourceService : IResourceServices
    {
        private readonly IResourceRepository resourceRepository;
        private readonly IProjectRepository projectRepository;
        private readonly IUnitOfWork unitOfWork;

        public ResourceService(IResourceRepository _resourceRepository, IProjectRepository _projectRepository, IUnitOfWork _unitOfWork)
        {
            resourceRepository = _resourceRepository;
            projectRepository = _projectRepository;
            unitOfWork = _unitOfWork;
        }

        public async Task<ResourceCreatedDto> Create(string name, Guid projectId, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("Resource name is required");
            var project = await projectRepository.GetById(projectId);
            if (project == null) throw new InvalidOperationException("Project not found");

            if (project.UserId != userId) throw new UnauthorizedAccessException("Access denied"); 

            var slug = GenerateSlug(name);
            var finalSlug = await EnsureUniqueSlugInProjectAsync(projectId, slug);

            var resource = new Resource
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Name = name,
                Slug = finalSlug,
                Count = 0
            };

            // 5. build default EndpointConfig — all enabled, pagination and search off
            var endpointConfig = new EndpointConfig
            {
                Id = Guid.NewGuid(),
                ResourceId = resource.Id,
                GetList = true,
                GetById = true,
                Post = true,
                Put = true,
                Delete = true,
                EnablePagination = false,
                EnableSearch = false
            };

            // 6. attach config to resource so EF saves both in one call
            resource.EndpointConfig = endpointConfig;

            await unitOfWork.Resources.Add(resource);
            await unitOfWork.Resources.SaveChanges();

            return ResourceCreatedDto.FromEntity(resource);

        }

        public Task Delete(Guid resourceId, Guid projectId)
        {
            throw new NotImplementedException();
        }


        // helper funcs
        private string GenerateSlug(string name)
        {
            return name.ToLower().Trim().Replace(" ", "-").Replace("_", "-");
        }

        private async Task<string> EnsureUniqueSlugInProjectAsync(Guid projectId, string slug)
        {
            var finalSlug = slug;
            var counter = 1;

            while (!await resourceRepository.IsSlugUniqueInProjectAsync(projectId, finalSlug))
            {
                finalSlug = $"{slug}-{counter}";
                counter++;
            }

            return finalSlug;
        }
    }
}