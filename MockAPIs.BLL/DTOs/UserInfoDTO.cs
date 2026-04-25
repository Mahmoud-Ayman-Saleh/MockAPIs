using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockAPIs.BLL.DTOs
{
    public class UserInfoDTO
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}