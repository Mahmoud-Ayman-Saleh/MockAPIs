using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockAPIs.BLL.DTOs
{
    public class UpdateFieldDto
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public string? FakerHint { get; set; }
        public bool IsRequired { get; set; }
    }
}