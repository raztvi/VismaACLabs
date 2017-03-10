using System.IO;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IBlobService
    {
        Task UploadBlobFromStream(string containerName, string blobPath, Stream stream, string fileContentType = null,
            bool overwrite = false);

        Task DeleteBlob(string containerName, string blobPath);

        Task<Stream> GetBlobStream(string containerName, string blobPath);

        Task<long> GetFolderSize(string containerName, string folderPath);

        Task<long> GetContainerSize(string containerName);

        Task CreateContainerIfNotExists(string containerName);

        Task DeleteFolder(string containerName, string folderPath);

        Task DeleteContainer(string containerName);

        Task<string> GetTemporaryUrl(string containerName, string blobPath, int minutesToLive = 10);
    }
}