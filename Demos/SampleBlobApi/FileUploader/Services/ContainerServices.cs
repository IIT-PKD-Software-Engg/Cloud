using Azure.Storage.Blobs;
using Azure.Storage;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using FileUploader.Models;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace FileUploader.Services
{
    public class ContainerServices
    {
        private readonly string _storageAccount = "secloudstorage";
        private readonly string _key = "wYQuV8Cxw1HYub+hMMIQ8WxqERWRL51HdpOwPCvdm268iGq1n47rL6oejHRGEyiJc3Wx2mttEPkU+AStem/zkA==";
        private readonly BlobServiceClient _blobServiceClient;

        public ContainerServices()
        {
            var credential = new StorageSharedKeyCredential(_storageAccount, _key);
            var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
            _blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
        }
        
        public  async Task<BlobContainerClient> CreateContainerAsync(string containerName)
        {
            try
            {
                BlobContainerClient container = await _blobServiceClient.CreateBlobContainerAsync(containerName);

                if (await container.ExistsAsync())
                {
                    Console.WriteLine("Created Container {0}", container.Name);
                    return container;
                }
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine("HTTP error code {0}: {1}", e.Status, e.ErrorCode);
                Console.WriteLine(e.Message);   
            }
            return null;
        }
        public async Task<List<ContainerDetails>> ListContainersAsync()
        {
            List<ContainerDetails> containers = new List<ContainerDetails>();

            try
            {
                var resultSegment = _blobServiceClient.GetBlobContainersAsync(BlobContainerTraits.Metadata, prefix: null, default)
                    .AsPages(default, 100);

                await foreach(var containerPage in resultSegment)
                {
                    foreach(var containerItem in containerPage.Values)
                    {
                        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerItem.Name);
                        BlobContainerProperties properties = await containerClient.GetPropertiesAsync();

                        containers.Add(new ContainerDetails
                        {
                            Name = containerItem.Name,
                            LastModified = properties.LastModified,
                            MetaData = properties.Metadata,
                            PublicAccess = properties.PublicAccess
                        });
                    }
                }
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Error listing containers: {ex.Message}");
                throw;
            }

            return containers;
        }
        public async Task DeleteContainerAsync(string containerName)
        {
            BlobContainerClient container = _blobServiceClient.GetBlobContainerClient(containerName);
            try
            {
                await container.DeleteIfExistsAsync();
            }
            catch(RequestFailedException e)
            {
                Console.WriteLine("HTTP error code {0}: {1}", e.Status, e.ErrorCode);
                Console.WriteLine(e.Message);
            }
            return;
        }
	}
}
