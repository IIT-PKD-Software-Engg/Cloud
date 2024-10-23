﻿/*using Azure.Storage.Blobs;
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

/******************************************************************************
* Filename    = FileDownload.cs
*
* Author      = Arnav Rajesh Kadu
*
* Product     = Cloud
* 
* Project     = Unnamed Software Project
*
* Description = To download files from cloud to local
*****************************************************************************/

using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FA1
{
    /// <summary>
    /// Class responsible for downloading files from an Azure Blob Storage container.
    /// </summary>
    public class FileDownload
    {
        // Constant to store the name of the environment variable for the Azure Storage connection string.
        private const string _connectionStringName = "AzureWebJobsStorage";

        /// <summary>
        /// Azure Function to download a file from an Azure Blob Storage container.
        /// The function is triggered via an HTTP GET request with anonymous authorization and a specific route.
        /// </summary>
        /// <param name="req">HTTP request data.</param>
        /// <param name="team">Path parameter representing the team (used as the container name).</param>
        /// <param name="filename">Path parameter representing the filename to download.</param>
        /// <param name="executionContext">Execution context for the function (used to access the logger).</param>
        /// <returns>An HTTP response with the file content or an error message.</returns>
        [Function("DownloadFile")]
        public static async Task<HttpResponseData> DownloadFile(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "download/{team}/{filename}")] HttpRequestData req,
            string team,
            string filename,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger<FileDownload>();
            logger.LogInformation($"Attempting to download file '{filename}' from container '{team}'.");

            // Get the connection string from the environment variables.
            var connectionString = Environment.GetEnvironmentVariable(_connectionStringName);
            if (string.IsNullOrEmpty(connectionString))
            {
                logger.LogError("Connection string not found");
                // Create an internal server error response indicating the missing connection string.
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("Storage connection string not configured");
                return errorResponse;
            }

            try
            {
                // Initialize a BlobServiceClient using the connection string and get a reference to the Blob container for the specified team.
                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(team.ToLowerInvariant());

                await containerClient.CreateIfNotExistsAsync();

                // Get a reference to the specific blob (file) using the filename.
                var blobClient = containerClient.GetBlobClient(filename);
                logger.LogInformation($"Blob URI: {blobClient.Uri}");

                if (await blobClient.ExistsAsync())
                {
                    logger.LogInformation($"File '{filename}' exists in the container '{team}'.");

                    var blobDownloadInfo = await blobClient.DownloadAsync();
                    // Create an HTTP OK response for successful file download.
                    var response = req.CreateResponse(HttpStatusCode.OK);

                    // Set the Content-Type header based on the blob's content type, or use a default if not available.
                    var contentType = blobDownloadInfo.Value.Details.ContentType ?? "application/octet-stream";
                    response.Headers.Add("Content-Type", contentType);
                    // Set the Content-Disposition header to indicate a file download.
                    response.Headers.Add("Content-Disposition", $"attachment; filename={filename}");

                    // Copy the blob's content to the HTTP response body (streaming the file to the user).
                    await blobDownloadInfo.Value.Content.CopyToAsync(response.Body);
                    return response;
                }

                logger.LogWarning($"File '{filename}' not found in the container '{team}'.");
                // Create a NotFound response if the file does not exist.
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                await notFoundResponse.WriteStringAsync($"File {filename} not found in {team} container.");
                return notFoundResponse;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error processing request for file '{filename}' in container '{team}'");
                // Create an internal server error response in case of an exception.
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error processing request: {ex.Message}");
                return errorResponse;
            }
        }
    }
}
