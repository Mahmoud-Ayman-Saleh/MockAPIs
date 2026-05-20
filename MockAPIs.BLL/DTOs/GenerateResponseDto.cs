using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockAPIs.BLL.DTOs
{
    public class GenerateResponseDto
    {
        public Guid ResourceId { get; set; }
        public int GeneratedCount { get; set; }
        public string? Message { get; set; }
    }
}