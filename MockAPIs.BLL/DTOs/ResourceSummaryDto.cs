using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.DAL.Models;

namespace MockAPIs.BLL.DTOs
{
    public class ResourceSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Count { get; set; }

        public static ResourceSummaryDto FromEntity(Resource resource)
        {
            return new ResourceSummaryDto
            {
                Id = resource.Id,
                Name = resource.Name,
                Slug = resource.Slug,
                Count = resource.Count
            };
        }
    }
}