using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyIdentityServer.Models
{
    public class ApplicationUser : IdentityUser
    {
        public override string Id { get => base.Id; set => base.Id = Guid.NewGuid().ToString(); }
    }
}
