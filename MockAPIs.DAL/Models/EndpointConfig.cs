using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockAPIs.DAL.Models
{
    public class EndpointConfig
    {
        public Guid Id { get; set; }
        public Guid ResourceId { get; set; }
        public Resource Resource { get; set; }
        public bool GetList { get; set; } = true;
        public bool GetById { get; set; } = true;
        public bool Post { get; set; } = true;
        public bool Put { get; set; } = true;
        public bool Delete { get; set; } = true;
        public bool EnablePagination { get; set; } = true;
        public bool EnableSearch { get; set; } = true;
    }
}