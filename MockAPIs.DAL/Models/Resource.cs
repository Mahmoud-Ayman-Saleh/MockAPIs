using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockAPIs.DAL.Models
{
    public class Resource
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Count { get; set; }
        public ICollection<Field> Fields { get; set; }
        public ICollection<MockRecord> MockRecords { get; set; }
        public EndpointConfig EndpointConfig { get; set; }
    }
}