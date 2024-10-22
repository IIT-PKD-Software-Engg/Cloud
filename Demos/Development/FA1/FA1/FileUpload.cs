using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using System;
using System.Net;
using System.Net.Http.Headers;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;

namespace FA1
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
            logger.LogInformation($"Uploading a file to the container: {team}");

            try
            {
                BlobContainerClient containerClient = new BlobContainerClient(
                    Environment.GetEnvironmentVariable(_connectionString),
                    team);

                // Ensure container exists
                await containerClient.CreateIfNotExistsAsync();

                // Read and process multipart form data
                if (!req.Headers.TryGetValues("Content-Type", out var contentTypes) ||
                    !contentTypes.First().Contains("multipart/form-data"))
                {
                    logger.LogWarning("Invalid content type received");
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
                    if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition) &&
                        contentDisposition.DispositionType.Equals("form-data") &&
                        contentDisposition.FileName != null)
                    {
                        // Clean the filename by removing quotes
                        var fileName = contentDisposition.FileName
                            ?.Trim('"')  // Remove surrounding quotes
                            ?.Replace("\\", "")  // Remove escape characters
                            ?.Trim();  // Remove any remaining whitespace

                        if (string.IsNullOrEmpty(fileName))
                        {
                            logger.LogWarning("Empty filename detected");
                            continue;
                        }

                        logger.LogInformation($"Processing file: {fileName}");

                        BlobClient blobClient = containerClient.GetBlobClient(fileName);

                        // Upload file to the blob
                        using (var stream = section.Body)
                        {
                            await blobClient.UploadAsync(stream, true);
                            logger.LogInformation($"File {fileName} uploaded successfully");
                        }

                        var response = req.CreateResponse(HttpStatusCode.OK);
                        await response.WriteStringAsync($"File {fileName} uploaded successfully to {team} container.");
                        return response;
                    }
                }

                // If no file section is found, return Bad Request
                logger.LogWarning("No valid file section found in request");
                var noFileResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await noFileResponse.WriteStringAsync("No valid file provided.");
                return noFileResponse;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error uploading file");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error uploading file: {ex.Message}");
                return errorResponse;
            }
        }
    }

    public static class MultipartRequestHelper
    {
        public static string GetBoundary(string contentType)
        {
            var elements = contentType.Split(' ');
            var boundaryElement = elements.FirstOrDefault(entry => entry.StartsWith("boundary="));
            var boundary = boundaryElement?.Substring("boundary=".Length).Trim('"');  // Remove quotes from boundary
            return boundary;
        }
    }
}