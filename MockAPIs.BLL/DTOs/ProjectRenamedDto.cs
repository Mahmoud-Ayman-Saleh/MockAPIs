using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.DAL.Models;

namespace MockAPIs.BLL.DTOs
{
    public class ProjectRenamedDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }

        public static ProjectRenamedDto FromEntity(Project project)
        {
            return new ProjectRenamedDto
            {
                Id = project.Id,
                Name = project.Name,
                Slug = project.Slug
            };
        }
    }
}