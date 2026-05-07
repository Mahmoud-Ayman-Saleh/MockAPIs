using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockAPIs.BLL.DTOs
{
    public class UpdateEndpointConfigDto
    {
        public bool GetList { get; set; }
        public bool GetById { get; set; }
        public bool Post { get; set; }
        public bool Put { get; set; }
        public bool Delete { get; set; }
        public bool EnablePagination { get; set; }
        public bool EnableSearch { get; set; }
    }
}