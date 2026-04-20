using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MockAPIs.DAL.Enums;

namespace MockAPIs.DAL.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
        public UserPlan Plan {get; set;} = UserPlan.Free;
        public ICollection<Project> Projects { get; set; }
        
    }
}