using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Core.Entities
{
    public class User : IdentityUser
    {
        public Guid CompanyId { get; set; }
    }
}