/*using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FA1
{
    public class FileDownload
    {
        private const string _connectionString = "AzureWebJobsStorage";

        [Function("DownloadFile")]
        public static async Task<HttpResponseData> DownloadFile(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "download/{team}/{filename}")] HttpRequestData req,
    string team,
    string filename,
    FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger<FileDownload>();
            logger.LogInformation($"Attempting to download file '{filename}' from container '{team}'.");

            var connectionString = Environment.GetEnvironmentVariable(_connectionString);
            logger.LogInformation($"Connection string: {connectionString}");

            BlobContainerClient containerClient = new BlobContainerClient(connectionString, team);
            BlobClient blobClient = containerClient.GetBlobClient(filename);

            logger.LogInformation($"Blob URI: {blobClient.Uri}");

            if (await blobClient.ExistsAsync())
            {
                logger.LogInformation($"File '{filename}' exists in the container '{team}'.");

                var blobDownloadInfo = await blobClient.DownloadAsync();
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", blobDownloadInfo.Value.Details.ContentType);
                response.Headers.Add("Content-Disposition", $"attachment; filename={filename}");

                await blobDownloadInfo.Value.Content.CopyToAsync(response.Body);
                return response;
            }

            logger.LogWarning($"File '{filename}' not found in the container '{team}'.");

            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync($"File {filename} not found in {team} container.");
            return notFoundResponse;
        }
    }
}
*/

using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FA1
{
    public class FileDownload
    {
        private const string _connectionStringName = "AzureWebJobsStorage";

        [Function("DownloadFile")]
        public static async Task<HttpResponseData> DownloadFile(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "download/{team}/{filename}")] HttpRequestData req,
            string team,
            string filename,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger<FileDownload>();
            logger.LogInformation($"Attempting to download file '{filename}' from container '{team}'.");

            var connectionString = Environment.GetEnvironmentVariable(_connectionStringName);
            if (string.IsNullOrEmpty(connectionString))
            {
                logger.LogError("Connection string not found");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("Storage connection string not configured");
                return errorResponse;
            }

            try
            {
                // Create container client with default options
                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(team.ToLowerInvariant());

                // Ensure container exists
                await containerClient.CreateIfNotExistsAsync();

                var blobClient = containerClient.GetBlobClient(filename);
                logger.LogInformation($"Blob URI: {blobClient.Uri}");

                if (await blobClient.ExistsAsync())
                {
                    logger.LogInformation($"File '{filename}' exists in the container '{team}'.");
                    var blobDownloadInfo = await blobClient.DownloadAsync();
                    var response = req.CreateResponse(HttpStatusCode.OK);

                    // Set content type if available, otherwise use application/octet-stream
                    var contentType = blobDownloadInfo.Value.Details.ContentType ?? "application/octet-stream";
                    response.Headers.Add("Content-Type", contentType);
                    response.Headers.Add("Content-Disposition", $"attachment; filename={filename}");

                    await blobDownloadInfo.Value.Content.CopyToAsync(response.Body);
                    return response;
                }

                logger.LogWarning($"File '{filename}' not found in the container '{team}'.");
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                await notFoundResponse.WriteStringAsync($"File {filename} not found in {team} container.");
                return notFoundResponse;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error processing request for file '{filename}' in container '{team}'");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error processing request: {ex.Message}");
                return errorResponse;
            }
        }
    }
}