using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace Data.Services
{
    public class AzureBlobService : IBlobService
    {
        private readonly CloudBlobClient _blobClient;

        public AzureBlobService(IConfiguration configuration)
        {
            // Get connection string
            var storageConnectionString = configuration.GetConnectionString("AzureStorageConnectionString");
            if (string.IsNullOrWhiteSpace(storageConnectionString))
                throw new Exception("Azure storage connection string is missing.");

            // Get account
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Get and set up client
            _blobClient = storageAccount.CreateCloudBlobClient();
            _blobClient.DefaultRequestOptions.StoreBlobContentMD5 = true;
            _blobClient.DefaultRequestOptions.SingleBlobUploadThresholdInBytes = 4194304; //4 MiB, current maximum
            //no need for parallelism, the input is being streamed (serially) by the client
            _blobClient.DefaultRequestOptions.ParallelOperationThreadCount = 1;
            _blobClient.DefaultRequestOptions.RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(1), 10);
        }

        /// <summary>
        ///     Uploads a blob from a stream
        /// </summary>
        /// <param name="containerName">Container name</param>
        /// <param name="blobPath">Blob path inside of the given container</param>
        /// <param name="stream">Source stream for blob content</param>
        /// <param name="fileContentType">File content type</param>
        /// <param name="overwrite">Optional parameter that enables overwriting an existing blob</param>
        /// <returns>An awaitable <see cref="Task" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="containerName" /> or <paramref name="blobPath" />
        ///     are null or empty strings.
        /// </exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="stream" /> is not readable.</exception>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when <paramref name="overwrite" /> is set to false and the
        ///     <paramref name="blobPath" /> references an existing blob.
        /// </exception>
        public async Task UploadBlobFromStream(string containerName, string blobPath, Stream stream,
            string fileContentType = null, bool overwrite = false)
        {
            // check parameters
            if (string.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));
            if (string.IsNullOrWhiteSpace(blobPath)) throw new ArgumentNullException(nameof(blobPath));
            if (!stream.CanRead) throw new ArgumentException("Provided stream is unreadable", nameof(stream));

            // get container reference and create if it doesn't exist yet
            var container = _blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            // get blob reference
            var blob = container.GetBlockBlobReference(blobPath);
            if (!overwrite && await blob.ExistsAsync())
                throw new InvalidOperationException(
                    $"Blob {blobPath} already exists. Set {nameof(overwrite)} to true if you want to overwrite it.");

            if (!string.IsNullOrWhiteSpace(fileContentType))
                blob.Properties.ContentType = fileContentType;
            //retry already configured of Azure Storage API
            await blob.UploadFromStreamAsync(stream);
        }

        /// <summary>
        ///     Removes a blob from Azure Storage
        /// </summary>
        /// <param name="containerName">Container name</param>
        /// <param name="blobPath">Blob path inside of the given container</param>
        /// <returns>Awaitable <see cref="Task" /></returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="containerName" /> or <paramref name="blobPath" />
        ///     are null or empty strings.
        /// </exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="containerName" /> container does not exist.</exception>
        public async Task DeleteBlob(string containerName, string blobPath)
        {
            // check parameters
            if (string.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));
            if (string.IsNullOrWhiteSpace(blobPath)) throw new ArgumentNullException(nameof(blobPath));

            // get and check container reference
            var container = _blobClient.GetContainerReference(containerName);
            if (!await container.ExistsAsync())
                throw new ArgumentException($"Container {container} does not exist");

            // get blob reference
            var blob = container.GetBlockBlobReference(blobPath);
            await blob.DeleteIfExistsAsync();
        }

        /// <summary>
        ///     Gets a blob's content stream
        /// </summary>
        /// <param name="containerName">Container name</param>
        /// <param name="blobPath">Blob path inside of the given container</param>
        /// <returns>The blob's content stream</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="containerName" /> or <paramref name="blobPath" />
        ///     are null or empty strings.
        /// </exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="containerName" /> container does not exist.</exception>
        public async Task<Stream> GetBlobStream(string containerName, string blobPath)
        {
            // check parameters
            if (string.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));
            if (string.IsNullOrWhiteSpace(blobPath)) throw new ArgumentNullException(nameof(blobPath));

            // get and check container reference
            var container = _blobClient.GetContainerReference(containerName);
            if (!await container.ExistsAsync())
                throw new ArgumentException($"Container {container} does not exist");

            // get blob reference
            var blob = container.GetBlockBlobReference(blobPath);
            if (await blob.ExistsAsync())
                return await blob.OpenReadAsync();

            return null;
        }

        /// <summary>
        ///     Gets the total size of a virtual folder.
        /// </summary>
        /// <param name="containerName">Container name</param>
        /// <param name="folderPath">Virtual folder path</param>
        /// <returns>The total size in bytes</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="containerName" /> or <paramref name="folderPath" />
        ///     are null or empty strings.
        /// </exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="containerName" /> container does not exist.</exception>
        public async Task<long> GetFolderSize(string containerName, string folderPath)
        {
            // check parameters
            if (string.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));
            if (string.IsNullOrWhiteSpace(folderPath)) throw new ArgumentNullException(nameof(folderPath));

            // get and check container reference
            var container = _blobClient.GetContainerReference(containerName);
            if (!await container.ExistsAsync())
                //throw new ArgumentException($"Container {container} does not exist");
                return 0;

            var folder = container.GetDirectoryReference(folderPath);
            var folderSize = (await
                    folder.ListBlobsSegmentedAsync(true, BlobListingDetails.All, null, null, null, null))
                .Results.Sum(x => (x as CloudBlob)?.Properties.Length ?? 0);

            return folderSize;
        }

        /// <summary>
        ///     Gets the total size of a container.
        /// </summary>
        /// <param name="containerName">Container name</param>
        /// <returns>The total size in bytes</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="containerName" /> is null or empty string.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="containerName" /> container does not exist.</exception>
        public async Task<long> GetContainerSize(string containerName)
        {
            // check parameters
            if (string.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));

            // get and check container reference
            var container = _blobClient.GetContainerReference(containerName);
            if (!await container.ExistsAsync())
                //throw new ArgumentException($"Container {container} does not exist");
                return 0;

            var containerSize = (await
                    container.ListBlobsSegmentedAsync(string.Empty, true, BlobListingDetails.All, null, null, null, null))
                .Results
                .Sum(x => (x as CloudBlob)?.Properties.Length ?? 0);

            return containerSize;
        }

        /// <summary>
        ///     Creates a container
        /// </summary>
        /// <param name="containerName">Container name</param>
        /// <returns>An awaitable <see cref="Task" /></returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="containerName" /> is null or empty string.</exception>
        public async Task CreateContainerIfNotExists(string containerName)
        {
            // check parameters
            if (string.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));

            var container = _blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
        }

        /// <summary>
        ///     Deletes all blobs in a virtual folder.
        /// </summary>
        /// <param name="containerName">Container name</param>
        /// <param name="folderPath">Virtual folder path</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="containerName" /> or <paramref name="folderPath" />
        ///     are null or empty strings.
        /// </exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="containerName" /> container does not exist.</exception>
        public async Task DeleteFolder(string containerName, string folderPath)
        {
            // check parameters
            if (string.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));
            if (string.IsNullOrWhiteSpace(folderPath)) throw new ArgumentNullException(nameof(folderPath));

            // get and check container reference
            var container = _blobClient.GetContainerReference(containerName);
            if (!await container.ExistsAsync())
                throw new ArgumentException($"Container {container} does not exist");

            var folder = container.GetDirectoryReference(folderPath);
            var blobs = (await
                folder.ListBlobsSegmentedAsync(true, BlobListingDetails.All, -1, null, null, null)).Results;
            foreach (var blob in blobs)
                await container.GetBlockBlobReference(((CloudBlockBlob) blob).Name).DeleteIfExistsAsync();
        }

        /// <summary>
        ///     Deletes a container and all its blobs
        /// </summary>
        /// <param name="containerName">Container name</param>
        /// <returns>An awaitable <see cref="Task" /></returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="containerName" /> is null or empty string.</exception>
        public async Task DeleteContainer(string containerName)
        {
            // check parameters
            if (string.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));

            // get and check container reference
            var container = _blobClient.GetContainerReference(containerName);
            await container.DeleteIfExistsAsync();
        }

        /// <summary>
        ///     Get SAS url
        /// </summary>
        /// <param name="containerName">Container name</param>
        /// <param name="blobPath">Blob path</param>
        /// <param name="minutesToLive">Minutes the SAS url is valid. Must be a positive, non zero integer.</param>
        /// <returns>The SAS url</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">
        ///     When the container specified by <paramref name="containerName" /> does not exist, or when
        ///     <paramref name="minutesToLive" /> is a negative or zero integer.
        /// </exception>
        public async Task<string> GetTemporaryUrl(string containerName, string blobPath, int minutesToLive = 10)
        {
            // check parameters
            if (string.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));
            if (string.IsNullOrWhiteSpace(blobPath)) throw new ArgumentNullException(nameof(blobPath));
            if (minutesToLive <= 0) throw new ArgumentException("Value cannot be 0 or below", nameof(minutesToLive));

            // get and check container reference
            var container = _blobClient.GetContainerReference(containerName);
            if (!await container.ExistsAsync())
                throw new ArgumentException($"Container {container} does not exist");

            var blob = container.GetBlobReference(blobPath);

            var sas = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.Now.AddMinutes(minutesToLive)
            });

            return blob.Uri.AbsoluteUri + sas;
        }
    }
}