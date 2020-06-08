using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApp.Models
{
    public class AppRole : IdentityRole
    {
        public AppRole(string name)
        {
            base.Name = name;
        }
    }
}
