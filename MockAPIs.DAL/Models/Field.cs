using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.DAL.Enums;

namespace MockAPIs.DAL.Models
{
    public class Field
    {
        public Guid Id { get; set; }
        public Guid ResourceId { get; set; }
        public Resource Resource { get; set; }
        public string Name { get; set; }
        public FieldDataType DataType { get; set; }
        public string? FakerHint { get; set; }
        public bool IsRequired { get; set; }
    }
}