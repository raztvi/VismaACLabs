using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;

namespace Core.Entities
{
    public class User : IdentityUser
    {
        public Guid CompanyId { get; set; }
    }
}
