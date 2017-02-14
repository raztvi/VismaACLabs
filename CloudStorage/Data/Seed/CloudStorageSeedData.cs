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
            await SeedFileInfos();
            await SeedCompanies();
        }

        private async Task SeedCompanies()
        {
            if (!_context.Companies.Any())
            {
                var comp1 = new Company
                {
                    ContactEmail = "someone@NoOneCanHearYouScream.com",
                    ContactPhoneNumber = "1234567890",
                    MainAddress = "Somewhere over the rainbow",
                    Name = "Weyland Yutani Corp"
                };
                var comp2 = new Company
                {
                    ContactEmail = "cr@rsi.com",
                    ContactPhoneNumber = "1234567890",
                    MainAddress = "Terra Prime",
                    Name = "Roberts Space Industries"
                };
                var comp3 = new Company
                {
                    ContactEmail = "vesimir@wolfs.org",
                    ContactPhoneNumber = "1234567890",
                    MainAddress = "Kaer Morhen",
                    Name = "School of the Wolf"
                };

                await _context.Companies.AddRangeAsync(comp1, comp2, comp3);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedFileInfos()
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

                await _context.FileInfos.AddRangeAsync(fileInfo1, fileInfo2, fileInfo3, fileInfo4, fileInfo5);
                await _context.SaveChangesAsync();
            }
        }
    }
}