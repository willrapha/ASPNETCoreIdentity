using System;
using Microsoft.AspNetCore.Identity;

namespace WithoutIdentity.Models
{
    public class ApplicationUser: IdentityUser<Guid>
    {
        public ApplicationUser()
        {
            
        }
    }
}