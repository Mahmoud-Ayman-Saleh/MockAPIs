using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using MockAPIs.BLL.DTOs;
using MockAPIs.BLL.Interfaces;
using MockAPIs.DAL.Interfaces;

namespace MockAPIs.BLL.Services
{
    public class ResourceService : IResourceServices
    {
        private readonly IResourceRepository resourceRepository;
        private readonly IUnitOfWork unitOfWork;

        public ResourceService(IResourceRepository _resourceRepository, IUnitOfWork _unitOfWork)
        {
            resourceRepository = _resourceRepository;
            unitOfWork = _unitOfWork;
        }

        public Task<ResourceCreated> Created(string name, Guid projectId)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Guid resourceId, Guid projectId)
        {
            throw new NotImplementedException();
        }


        // helper functions
        // Helper functions 
        private string GenerateSlug(string name)
        {
            return name.ToLower().Trim().Replace(" ", "-").Replace("_", "-");
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