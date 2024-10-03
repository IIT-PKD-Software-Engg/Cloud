using Azure.Storage;
using Azure.Storage.Blobs;
using FileUploader.Models;

namespace FileUploader.Services
{
    public class FileServices
    {
        private readonly string _storageAccount = "secloudstorage";
        private readonly string _key = "wYQuV8Cxw1HYub+hMMIQ8WxqERWRL51HdpOwPCvdm268iGq1n47rL6oejHRGEyiJc3Wx2mttEPkU+AStem/zkA==";
        private readonly BlobContainerClient _filesContainer;

        public FileServices()
        {
            var credential = new StorageSharedKeyCredential(_storageAccount, _key);
            var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
            var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
            _filesContainer = blobServiceClient.GetBlobContainerClient("testblobcontainer");
        }

        public async Task<List<BlobDetails>> ListAsync()
        {
            List<BlobDetails> files = new List<BlobDetails>();

            await foreach(var file in _filesContainer.GetBlobsAsync())
            {
                string uri = _filesContainer.Uri.ToString();
                var name = file.Name;
                var fullUri = $"{uri}/{name}";

                files.Add(new BlobDetails
                {
                    Uri = fullUri,
                    Name = name,
                    ContentType = file.Properties.ContentType
                });
            }
            return files;
        }

        public async Task<BlobResponseDto> UploadAsync(IFormFile blob)
        {
            BlobResponseDto response = new();
            BlobClient client = _filesContainer.GetBlobClient(blob.FileName);

            await using (Stream? data = blob.OpenReadStream())
            {
                await client.UploadAsync(data);
            }

            response.Status = $"File {blob.FileName} Uploaded Successfully";
            response.Error = false;
            response.Blob.Uri = client.Uri.AbsoluteUri;
            response.Blob.Name = client.Name;

            return response;
        }

        public async Task<BlobDetails?> DownloadAsync(string blobFilename)
        {
            BlobClient file = _filesContainer.GetBlobClient(blobFilename);

            if (await file.ExistsAsync())
            {
                var data = await file.OpenReadAsync();
                Stream blobContent = data;

                var content = await file.DownloadContentAsync();

                string name = blobFilename;
                string contentType = content.Value.Details.ContentType;

                return new BlobDetails
                {
                    Content = blobContent,
                    Name = name,
                    ContentType = contentType
                };
            }
            return null;
        }

        public async Task<BlobResponseDto> DeleteAsync(string blobFilename)
        {
            BlobClient file = _filesContainer.GetBlobClient(blobFilename);
            await file.DeleteAsync();

            return new BlobResponseDto
            {
                Error = false,
                Status = $"File: {blobFilename} has been successfully deleted."
            };
        }
    }
}
