using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.DAL.Models;

namespace MockAPIs.BLL.DTOs
{
    public class ProjectDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Token { get; set; }
        public string BaseUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ResourceSummaryDto> Resources { get; set; }

        public static ProjectDetailsDto FromEntity(Project project)
        {
            return new ProjectDetailsDto
            {
                Id = project.Id,
                Name = project.Name,
                Slug = project.Slug,
                Token = project.Token,
                BaseUrl = project.BaseUrl,
                IsActive = project.IsActive,
                CreatedAt = project.CreatedAt,
                Resources = project.Resources?.Select(ResourceSummaryDto.FromEntity).ToList() ?? new List<ResourceSummaryDto>()
            };
        }
    }
}