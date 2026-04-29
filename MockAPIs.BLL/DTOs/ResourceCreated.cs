using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.DAL.Models;

namespace MockAPIs.BLL.DTOs
{
    public class ResourceCreated
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public int Count { get; set; }
        public EndpointConfigDto EndpointConfig { get; set; }

        public static ResourceCreated FromEntity(Resource resource)
        {
            return new ResourceCreated
            {
                Id = resource.Id,
                Name = resource.Name,
                Slug = resource.Slug,
                Count = resource.Count,
                EndpointConfig = EndpointConfigDto.FromEntity(resource.EndpointConfig)
            };
        }
    }
}