using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockAPIs.DAL.Models
{
    public class MockRecord
    {
        public Guid Id { get; set; }
        public Guid ResourceId { get; set; }
        public Resource Resource { get; set; }
        public string Data { get; set; }          // stored as JSON string (JSONB in PostgreSQL)
        public DateTime CreatedAt { get; set; }
    }
}