using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class CloudStorageDbContext : DbContext
    {
        public DbSet<FileInfo> FileInfos { get; set; }

        public CloudStorageDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}
