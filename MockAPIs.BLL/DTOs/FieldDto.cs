using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.DAL.Models;

namespace MockAPIs.BLL.DTOs
{
    public class FieldDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }
        public string? FakerHint { get; set; }
        public bool IsRequired { get; set; }

        public static FieldDto FromEntity(Field field)
        {
            return new FieldDto
            {
                Id = field.Id,
                Name = field.Name,
                DataType = field.DataType.ToString(),
                FakerHint = field.FakerHint,
                IsRequired = field.IsRequired
            };
        }
    }
}