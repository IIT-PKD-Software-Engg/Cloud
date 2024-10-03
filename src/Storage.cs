/******************************************************************************
* Filename    = Storage.cs
*
* Author      = Pranav Guruprasad Rao
*
* Product     = Unnamed-Software-Engineering-Project
* 
* Project     = Cloud
*
* Description = Actual Storage Class
*****************************************************************************/

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


public class BlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobContainerClient _blobContainerClient;

    public BlobService(string containerName)
    {
        var _blobServiceClient = new BlobServiceClient(
        new Uri("https://secloudstorage.blob.core.windows.net"),
        new DefaultAzureCredential());

        var _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
    }

    public async Task CreateContainerAsync(string containerName)
    {
        string _containerName = containerName + Guid.NewGuid().ToString();
        _blobContainerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName);
    }

    public async Task<IEnumerable<string>> ListContainersAsync()
    {
        var containers = _blobServiceClient.GetBlobContainersAsync();
        return containers.Select(c => c.Name).ToListAsync();
    }

    public async Task<IEnumerable<string>> ListBlobsAsync()
    {
        var blobs = _blobContainerClient.GetBlobs();
        return blobs.Select(b => b.Name).ToListAsync();
    }

    public async Task DeleteBlobAsync(string blobName)
    {
        await _blobContainerClient.DeleteBlobIfExistsAsync(blobName);
    }

    public async Task ClearContainerAsync(string containerName)
    {
        await foreach(var blobItem in ListBlobsAsync())
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobItem);
            await blobClient.DeleteIfExistsAsync();
        }
    }

    public async Task DeleteContainerAsync()
    {
        await _blobContainerClient.DeleteIfExistsAsync();
    }

    public async Task<string> UploadBlobAsync(stream fileStream, string blobName)
    {
        BlobClient blobUploadClient = containerClient.GetBlobClient(blobName);
        await blobUploadClient.UploadAsync(fileStream, true);
        return blobUploadClient.Uri.ToString();
    }

    public async Task<Stream> DownloadBlobAsync(string blobName)
    {
        BlobClient blobDownloadClient = containerClient.GetBlobClient(blobName);
        var fileDownload = await blobDownloadClient.DownloadToAsync();
        return fileDownload.Value.Content;
    }
}