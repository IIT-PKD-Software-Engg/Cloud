/******************************************************************************
* Filename    = Cloud.cs
*
* Author      = Pranav Guruprasad Rao
*
* Product     = Unnamed-Software-Engineering-Project
* 
* Project     = Cloud
*
* Description = Cloud Abstraction Class
*****************************************************************************/

using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Cloud : ICloud
{
    private readonly BlobService _blobService;
    private readonly IDatabase _database;

    public Cloud(string containerName, IDatabase database)
    {
        _blobService = new BlobService(containerName);
        _database = database;
    }

    public async Task<Dictionary<string, object>> Get(Dictionary<string, string> userDetails, string folder, string dataUri)
    {
        try
        {
            var stream = await _blobService.DownloadBlobAsync($"{folder}/{dataUri}");
            using (var streamReader = new StreamReader(stream))
            {
                var content = await streamReader.ReadToEndAsync();
                return new Dictionary<string, object> { { "content", content } };
            }
        }
        catch (Exception ex)
        {
            // Handle and log the exception
            Console.WriteLine($"Error in Get: {ex.Message}");
            return null;
        }
    }

    public async Task<string> Post(Dictionary<string, string> userDetails, string folder, object data)
    {
        try
        {
            string fileName = Guid.NewGuid().ToString();
            string blobName = $"{folder}/{fileName}";

            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                await writer.WriteAsync(data.ToString());
                await writer.FlushAsync();
                stream.Position = 0;

                string uri = await _blobService.UploadBlobAsync(stream, blobName);
                return uri;
            }
        }
        catch (Exception ex)
        {
            // Handle and log the exception
            Console.WriteLine($"Error in Post: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> Put(Dictionary<string, string> userDetails, string folder, string oldDataUri, object data)
    {
        try
        {
            // Delete the old blob
            await _blobService.DeleteBlobAsync($"{folder}/{oldDataUri}");

            // Upload the new blob
            string newFileName = Guid.NewGuid().ToString();
            string newBlobName = $"{folder}/{newFileName}";

            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                await writer.WriteAsync(data.ToString());
                await writer.FlushAsync();
                stream.Position = 0;

                await _blobService.UploadBlobAsync(stream, newBlobName);
            }

            return true;
        }
        catch (Exception ex)
        {
            // Handle and log the exception
            Console.WriteLine($"Error in Put: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> Delete(Dictionary<string, string> userDetails, string folder, string dataUri)
    {
        try
        {
            await _blobService.DeleteBlobAsync($"{folder}/{dataUri}");
            return true;
        }
        catch (Exception ex)
        {
            // Handle and log the exception
            Console.WriteLine($"Error in Delete: {ex.Message}");
            return false;
        }
    }
}