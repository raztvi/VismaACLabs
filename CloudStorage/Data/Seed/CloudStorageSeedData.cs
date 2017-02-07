using Core.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Seed
{
    public class CloudStorageSeedData
    {
        private CloudStorageDbContext _context;

        public CloudStorageSeedData(CloudStorageDbContext context)
        {
            _context = context;
        }

        public async Task EnsureSeedData()
        {
            if (!_context.FileInfos.Any())
            {
                var fileInfo1 = new FileInfo
                {
                    ContentType = FileContentType.Invoice,
                    FileName = "Demo invoice.pdf",
                    FileSizeInBytes = 1234
                };

                var fileInfo2 = new FileInfo
                {
                    ContentType = FileContentType.Report,
                    FileName = "Bug report.csv",
                    FileSizeInBytes = 1234
                };

                var fileInfo3 = new FileInfo
                {
                    ContentType = FileContentType.Selfie,
                    FileName = "My last fire show.jpg",
                    FileSizeInBytes = 1234
                };

                var fileInfo4 = new FileInfo
                {
                    ContentType = FileContentType.Voucher,
                    FileName = "Something.png",
                    FileSizeInBytes = 1234
                };

                var fileInfo5 = new FileInfo
                {
                    ContentType = FileContentType.Other,
                    FileName = "MyNotes.txt",
                    FileSizeInBytes = 1234
                };

                _context.FileInfos.AddRange(fileInfo1, fileInfo2, fileInfo3, fileInfo4, fileInfo5);
                await _context.SaveChangesAsync();
            }
        }
    }
}