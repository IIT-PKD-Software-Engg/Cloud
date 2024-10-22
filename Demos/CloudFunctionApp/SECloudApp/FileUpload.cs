using Azure.Storage.Blobs;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SECloudApp
{
    public class FileUpload
    {
        private const string _connectionString = "AzureWebJobsStorage";

        [Function("UploadFile")]
        public static async Task<HttpResponseData> UploadFile(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "upload/{team}")] HttpRequestData req,
            string team,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger<FileUpload>();
            logger.LogInformation("Uploading a file to the container.");

            BlobContainerClient containerClient = new BlobContainerClient(Environment.GetEnvironmentVariable(_connectionString), team);

            // Read and process multipart form data
            if (!req.Headers.TryGetValues("Content-Type", out var contentTypes) || !contentTypes.First().Contains("multipart/form-data"))
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync("Invalid content type. Expecting multipart/form-data.");
                return badRequestResponse;
            }

            // Parse the multipart form data
            var boundary = MultipartRequestHelper.GetBoundary(contentTypes.First());
            var reader = new MultipartReader(boundary, req.Body);
            MultipartSection section;

            while ((section = await reader.ReadNextSectionAsync()) != null)
            {
                if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition) && contentDisposition.DispositionType.Equals("form-data") && contentDisposition.FileName!=null)
                {
                    // We have a file section, so we process it
                    var fileName = contentDisposition.FileName;
                    BlobClient blobClient = containerClient.GetBlobClient(fileName);

                    // Upload file to the blob
                    using (var stream = section.Body)
                    {
                        await blobClient.UploadAsync(stream, true);
                    }

                    var response = req.CreateResponse(HttpStatusCode.OK);
                    await response.WriteStringAsync($"File {fileName} uploaded successfully to {team} container.");
                    return response;
                }
            }

            // If no file section is found, return Bad Request
            var noFileResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await noFileResponse.WriteStringAsync("No valid file provided.");
            return noFileResponse;
        }
    }

    public static class MultipartRequestHelper
    {
        public static string GetBoundary(string contentType)
        {
            var elements = contentType.Split(' ');
            var boundaryElement = elements.FirstOrDefault(entry => entry.StartsWith("boundary="));
            var boundary = boundaryElement?.Substring("boundary=".Length);
            return boundary;
        }
    }
}
