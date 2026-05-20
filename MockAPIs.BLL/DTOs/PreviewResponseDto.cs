using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockAPIs.BLL.DTOs
{
    public class PreviewResponseDto
    {
        public Guid ResourceId { get; set; }
        public List<Dictionary<string, object?>> Preview { get; set; }
    }
}