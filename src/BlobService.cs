/******************************************************************************
* Filename    = BlobService.cs
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

    /// <summary>
    /// Create Container for the Account
    /// </summary>
    /// <param name="containerName">Container Name</param>
    public async Task CreateContainerAsync(string containerName)
    {
        string _containerName = containerName + Guid.NewGuid().ToString();
        _blobContainerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName);
    }

	/// <summary>
    /// List all Containers present in the Account
    /// </summary>
    public async Task<IEnumerable<string>> ListContainersAsync()
    {
        var containers = _blobServiceClient.GetBlobContainersAsync();
        return containers.Select(c => c.Name).ToListAsync();
    }

	/// <summary>
    /// List all Blobs present in the Container
    /// </summary>
    public async Task<IEnumerable<string>> ListBlobsAsync()
    {
        var blobs = _blobContainerClient.GetBlobs();
        return blobs.Select(b => b.Name).ToListAsync();
    }

	/// <summary>
    /// Delete Blob in the container
    /// </summary>
    /// <param name="blobName">Blob Name</param>
    public async Task DeleteBlobAsync(string blobName)
    {
        await _blobContainerClient.DeleteBlobIfExistsAsync(blobName);
    }

	/// <summary>
    /// Clear the current Container
    /// </summary>
    public async Task ClearContainerAsync()
    {
        await foreach(var blobItem in ListBlobsAsync())
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobItem);
            await blobClient.DeleteIfExistsAsync();
        }
    }

	/// <summary>
    /// Delete the current Container
    /// </summary>
    public async Task DeleteContainerAsync()
    {
        await _blobContainerClient.DeleteIfExistsAsync();
    }

	/// <summary>
    /// Upload a File as a Blob inside the container
    /// </summary>
    /// <param name="fileStream">File Contents</param>
    /// <param name="blobName">File Name</param>
    public async Task<string> UploadBlobAsync(stream fileStream, string blobName)
    {
        BlobClient blobUploadClient = _blobContainerClient.GetBlobClient(blobName);
        await blobUploadClient.UploadAsync(fileStream, true);
        return blobUploadClient.Uri.ToString();
    }

	/// <summary>
    /// Download a blob as a File inside the container
    /// </summary>
    /// <param name="blobName">File Name</param>
    public async Task<Stream> DownloadBlobAsync(string blobName)
    {
        BlobClient blobDownloadClient = _blobContainerClient.GetBlobClient(blobName);
        var fileDownload = await blobDownloadClient.DownloadToAsync();
        return fileDownload.Value.Content;
    }
}