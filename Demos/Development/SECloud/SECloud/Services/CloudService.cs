using Azure.Storage.Blobs;
using SECloud.Interfaces;
using SECloud.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SECloud.Services
{
    public class CloudService : ICloud
    {
        private readonly BlobContainerClient _containerClient;

        public CloudService(string sasUrl)
        {
            _containerClient = new BlobContainerClient(new Uri(sasUrl));
            _containerClient.CreateIfNotExists();
        }

        // Upload a blob
        public async Task<ServiceResponse<string>> UploadAsync(string blobName, Stream content, string contentType)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(content, overwrite: true);

            return new ServiceResponse<string>
            {
                Success = true,
                Data = blobClient.Uri.ToString()
            };
        }

        // Download a blob
        public async Task<ServiceResponse<Stream>> DownloadAsync(string blobName)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            var downloadStream = await blobClient.OpenReadAsync();

            return new ServiceResponse<Stream>
            {
                Success = true,
                Data = downloadStream
            };
        }

        // Delete a blob
        public async Task<ServiceResponse<bool>> DeleteAsync(string blobName)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();

            return new ServiceResponse<bool> { Success = true, Data = true };
        }

        // List blobs in a container
        public async Task<ServiceResponse<List<string>>> ListBlobsAsync(string prefix)
        {
            var results = new List<string>();
            await foreach (var blob in _containerClient.GetBlobsAsync(prefix: prefix))
            {
                results.Add(blob.Name);
            }

            return new ServiceResponse<List<string>> { Success = true, Data = results };
        }

        // Retrieve configuration blob (e.g., settings)
        public async Task<ServiceResponse<string>> RetrieveConfigAsync(string blobName)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            var configContent = await blobClient.DownloadContentAsync();

            return new ServiceResponse<string>
            {
                Success = true,
                Data = configContent.Value.Content.ToString()
            };
        }

        public Task<ServiceResponse<string>> UpdateAsync(string blobName)
        {
            throw new NotImplementedException();
        }
    }
}