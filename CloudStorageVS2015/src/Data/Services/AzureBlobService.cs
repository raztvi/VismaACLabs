using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace Data.Services
{
    public class AzureBlobService
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudBlobClient _blobClient;

        public AzureBlobService(IConfiguration configuration)
        {
            // Get connection string
            var storageConnectionString = configuration.GetConnectionString("AzureStorageConnectionString");
            if (string.IsNullOrWhiteSpace(storageConnectionString)) throw new Exception("Azure storage connection string is missing.");

            // Get account
            _storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Get and set up client
            _blobClient = _storageAccount.CreateCloudBlobClient();
            _blobClient.DefaultRequestOptions.StoreBlobContentMD5 = true;
            _blobClient.DefaultRequestOptions.SingleBlobUploadThresholdInBytes = 4194304; //4 MiB, current maximum
            //no need for parallelism, the input is being streamed (serially) by the client
            _blobClient.DefaultRequestOptions.ParallelOperationThreadCount = 1;
            _blobClient.DefaultRequestOptions.RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(1), 10);
        }

        /// <summary>
        /// Uploads a blob from a stream
        /// </summary>
        /// <param name="containerName">Container name</param>
        /// <param name="blobPath">Blob path inside of the given container</param>
        /// <param name="stream">Source stream for blob content</param>
        /// <param name="fileContentType">File content type</param>
        /// <param name="overwrite">Optional parameter that enables overwriting an existing blob</param>
        /// <returns> <see cref="BlobProperties"/> The uploaded blob's properties.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="containerName"/> or <paramref name="blobPath"/> are null or empty strings.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="stream"/> is not readable.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="overwrite" /> is set to false and the <paramref name="blobPath"/> references an existing blob.</exception>
        public async Task<BlobProperties> UploadBlobFromStream(string containerName, string blobPath, Stream stream,
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
            {
                throw new InvalidOperationException($"Blob {blobPath} already exists. Set {nameof(overwrite)} to true if you want to overwrite it.");
            }

            if (!string.IsNullOrWhiteSpace(fileContentType))
            {
                blob.Properties.ContentType = fileContentType;
            }
            //retry already configured of Azure Storage API
            await blob.UploadFromStreamAsync(stream);
            return blob.Properties;
        }

        /// <summary>
        /// Removes a blob from Azure Storage
        /// </summary>
        /// <param name="containerName">Container name</param>
        /// <param name="blobPath">Blob path inside of the given container</param>
        /// <returns>Awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="containerName"/> or <paramref name="blobPath"/> are null or empty strings.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="containerName"/> container does not exist.</exception>
        public async Task DeleteBlob(string containerName, string blobPath)
        {
            // check parameters
            if (string.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));
            if (string.IsNullOrWhiteSpace(blobPath)) throw new ArgumentNullException(nameof(blobPath));

            // get and check container reference
            var container = _blobClient.GetContainerReference(containerName);
            if (!await container.ExistsAsync())
            {
                throw new ArgumentException($"Container {container} does not exist");
            }

            // get blob reference
            var blob = container.GetBlockBlobReference(blobPath);
            await blob.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Gets a blobs properties.
        /// </summary>
        /// <param name="containerName">Container name</param>
        /// <param name="blobPath">Blob path inside of the given container</param>
        /// <returns><see cref="BlobProperties"/> of the blob, or null if blob does not exist</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="containerName"/> or <paramref name="blobPath"/> are null or empty strings.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="containerName"/> container does not exist.</exception>
        public async Task<BlobProperties> GetBlobProperties(string containerName, string blobPath)
        {
            // check parameters
            if (string.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));
            if (string.IsNullOrWhiteSpace(blobPath)) throw new ArgumentNullException(nameof(blobPath));

            // get and check container reference
            var container = _blobClient.GetContainerReference(containerName);
            if (!await container.ExistsAsync())
            {
                throw new ArgumentException($"Container {container} does not exist");
            }

            // get blob reference
            var blob = container.GetBlockBlobReference(blobPath);
            if (await blob.ExistsAsync())
            {
                await blob.FetchAttributesAsync();
                return blob.Properties;
            }

            return null;
        }

        /// <summary>       
        /// Gets a blob's content stream
        /// </summary>
        /// <param name="containerName">Container name</param>
        /// <param name="blobPath">Blob path inside of the given container</param>
        /// <returns>The blob's content stream</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="containerName"/> or <paramref name="blobPath"/> are null or empty strings.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="containerName"/> container does not exist.</exception>
        public async Task<Stream> GetBlobStream(string containerName, string blobPath)
        {
            // check parameters
            if (string.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));
            if (string.IsNullOrWhiteSpace(blobPath)) throw new ArgumentNullException(nameof(blobPath));

            // get and check container reference
            var container = _blobClient.GetContainerReference(containerName);
            if (!await container.ExistsAsync())
            {
                throw new ArgumentException($"Container {container} does not exist");
            }

            // get blob reference
            var blob = container.GetBlockBlobReference(blobPath);
            if (await blob.ExistsAsync())
            {
                return await blob.OpenReadAsync();
            }

            return null;
        }

        /// <summary>
        /// Gets the total size of a virtual folder.
        /// </summary>
        /// <param name="containerName">Container name</param>
        /// <param name="folderPath">Virtual folder path</param>
        /// <returns>The total size in bytes</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="containerName"/> or <paramref name="folderPath"/> are null or empty strings.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="containerName"/> container does not exist.</exception>
        public async Task<long> GetFolderSize(string containerName, string folderPath)
        {
            // check parameters
            if (string.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));
            if (string.IsNullOrWhiteSpace(folderPath)) throw new ArgumentNullException(nameof(folderPath));

            // get and check container reference
            var container = _blobClient.GetContainerReference(containerName);
            if (!await container.ExistsAsync())
            {
                throw new ArgumentException($"Container {container} does not exist");
            }

            var folder = container.GetDirectoryReference(folderPath);
            var folderSize =
                folder.ListBlobs(useFlatBlobListing: true).Sum(x => (x as CloudBlob)?.Properties.Length ?? 0);

            return folderSize;
        }

        /// <summary>
        /// Gets the total size of a container.
        /// </summary>
        /// <param name="containerName">Container name</param>
        /// <returns>The total size in bytes</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="containerName"/> is null or empty string.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="containerName"/> container does not exist.</exception>
        public async Task<long> GetContainerSize(string containerName)
        {
            // check parameters
            if (string.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));

            // get and check container reference
            CloudBlobClient container = _blobClient.GetContainerReference(containerName);
            if (!await container.ExistsAsync())
            {
                throw new ArgumentException($"Container {container} does not exist");
            }

            var containerSize =
                container.list ListBlobs(useFlatBlobListing: true).Sum(x => (x as CloudBlob)?.Properties.Length ?? 0);

            return containerSize;
        }

        /// <summary>
        /// Creates a container
        /// </summary>
        /// <param name="containerName">Container name</param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="containerName"/> is null or empty string.</exception>
        public async Task CreateContainer(string containerName)
        {
            // check parameters
            if (string.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));

            // get and check container reference
            var container = _blobClient.GetContainerReference(containerName);
            if (await container.ExistsAsync())
            {
                throw new ArgumentException($"Container {container} already exists");
            }

            await container.CreateAsync();
        }

        /// <summary>
        /// Deletes all blobs in a virtual folder.
        /// </summary>
        /// <param name="containerName">Container name</param>
        /// <param name="folderPath">Virtual folder path</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="containerName"/> or <paramref name="folderPath"/> are null or empty strings.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="containerName"/> container does not exist.</exception>
        public async Task DeleteFolder(string containerName, string folderPath)
        {
            // check parameters
            if (string.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));
            if (string.IsNullOrWhiteSpace(folderPath)) throw new ArgumentNullException(nameof(folderPath));

            // get and check container reference
            var container = _blobClient.GetContainerReference(containerName);
            if (!await container.ExistsAsync())
            {
                throw new ArgumentException($"Container {container} does not exist");
            }

            var folder = container.GetDirectoryReference(folderPath);
            foreach (var blob in folder.ListBlobs(useFlatBlobListing: true))
            {
                await container.GetBlockBlobReference(((CloudBlockBlob)blob).Name).DeleteIfExistsAsync();
            }
        }

        /// <summary>
        /// Deletes a container and all its blobs
        /// </summary>
        /// <param name="containerName">Container name</param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="containerName"/> is null or empty string.</exception>
        public async Task DeleteContainer(string containerName)
        {
            // check parameters
            if (string.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));

            // get and check container reference
            var container = _blobClient.GetContainerReference(containerName);
            await container.DeleteIfExistsAsync();
        }


        public async Task<string> GetTemporaryUrl(string containerName, string blobPath, int minutesToLive = 10)
        {
            // check parameters
            if (string.IsNullOrWhiteSpace(containerName)) throw new ArgumentNullException(nameof(containerName));
            if (string.IsNullOrWhiteSpace(blobPath)) throw new ArgumentNullException(nameof(blobPath));
            if (minutesToLive <= 0) throw new ArgumentException("Value cannot be 0 or below", nameof(minutesToLive));

            // get and check container reference
            var container = _blobClient.GetContainerReference(containerName);
            if (!await container.ExistsAsync())
            {
                throw new ArgumentException($"Container {container} does not exist");
            }

            CloudBlob blob = container.GetBlobReference(blobPath);

            var sas = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.Now.AddMinutes(minutesToLive)
            });

            return blob.Uri.AbsoluteUri + sas;
        }
    }
}
