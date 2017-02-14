using Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class CloudStorageDbContext : IdentityDbContext<User>
    {
        public DbSet<FileInfo> FileInfos { get; set; }
        public DbSet<Company> Companies { get; set; }

        public CloudStorageDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}
