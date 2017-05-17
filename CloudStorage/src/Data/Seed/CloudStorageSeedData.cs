using System.Linq;
using System.Threading.Tasks;
using Core.Entities;

namespace Data.Seed
{
    public class CloudStorageSeedData
    {
        private readonly CloudStorageDbContext _context;

        private readonly string _fileDescription1 = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque sagittis augue eget diam laoreet feugiat. Phasellus elementum scelerisque molestie. Curabitur consectetur porta arcu sit amet ullamcorper.";
        private readonly string _fileDescription2 = "At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga.";


        public CloudStorageSeedData(CloudStorageDbContext context)
        {
            _context = context;
        }

        public async Task EnsureSeedData()
        {
            await SeedCompanies();
            await SeedFileInfos();
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
                    ContactEmail = "vesimir@wolfs2.org",
                    ContactPhoneNumber = "1234567890",
                    MainAddress = "Kaer Morhen",
                    Name = "School of the Wolf"
                };
                var comp4 = new Company
                {
                    ContactEmail = "vlad@ap.net",
                    ContactPhoneNumber = "49231244",
                    MainAddress = "Mark Twa",
                    Name = "Dumdum"
                };

                await _context.Companies.AddRangeAsync(comp1, comp2, comp3, comp4);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedFileInfos()
        {
            if (!_context.FileInfos.Any())
            {
                var companyId = (_context.Companies.ToArray())[0].Id.ToString();

                var fileInfo1 = new FileInfo
                {
                    ContentType = FileContentType.Invoice,
                    FileName = "(dummy!) Demo invoice.pdf",
                    FileSizeInBytes = 1235,
                    ContainerName = companyId,
                    Description = _fileDescription1
                };
                
                var fileInfo2 = new FileInfo
                {
                    ContentType = FileContentType.Report,
                    FileName = "(dummy2!) Bug report.csv",
                    FileSizeInBytes = 1234,
                    ContainerName = companyId,
                    Description = _fileDescription2
                };
                
                var fileInfo3 = new FileInfo
                {
                    ContentType = FileContentType.Selfie,
                    FileName = "(dummy!) My last fire show.jpg",
                    FileSizeInBytes = 1234,
                    ContainerName = companyId,
                    Description = _fileDescription1
                };

                var fileInfo4 = new FileInfo
                {
                    ContentType = FileContentType.Voucher,
                    FileName = "(dummy!) Something.png",
                    FileSizeInBytes = 1234,
                    ContainerName = companyId,
                    Description = _fileDescription2
                };

                var fileInfo5 = new FileInfo
                {
                    ContentType = FileContentType.Voucher,
                    FileName = "(dummy!) MyNotes.txt",
                    FileSizeInBytes = 1234,
                    ContainerName = companyId,
                    Description = _fileDescription1
                };

                await _context.FileInfos.AddRangeAsync(fileInfo1, fileInfo2, fileInfo3, fileInfo4, fileInfo5);
                await _context.SaveChangesAsync();
            }
        }
    }
}