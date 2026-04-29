using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPIs.DAL.Models;

namespace MockAPIs.BLL.DTOs
{
    public class EndpointConfigDto
    {
        public bool GetList { get; set; }
        public bool GetById { get; set; }
        public bool Post { get; set; }
        public bool Put { get; set; }
        public bool Delete { get; set; }
        public bool EnablePagination { get; set; }
        public bool EnableSearch { get; set; }

        public static EndpointConfigDto FromEntity(EndpointConfig config)
        {
            return new EndpointConfigDto
            {
                GetList = config.GetList,
                GetById = config.GetById,
                Post = config.Post,
                Put = config.Put,
                Delete = config.Delete,
                EnablePagination = config.EnablePagination,
                EnableSearch = config.EnableSearch
            };
        }
    }
}