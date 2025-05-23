using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace SECloudApp
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
            logger.LogInformation($"Downloading a file {filename} from {team} container.");

            BlobContainerClient containerClient = new BlobContainerClient(Environment.GetEnvironmentVariable(_connectionString), team);
            BlobClient blobClient = containerClient.GetBlobClient(filename);

            if (await blobClient.ExistsAsync())
            {
                var blobDownloadInfo = await blobClient.DownloadAsync();

                // Create the response and set the appropriate headers
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", blobDownloadInfo.Value.Details.ContentType);
                response.Headers.Add("Content-Disposition", $"attachment; filename={filename}");

                // Write the blob content to the response stream
                await blobDownloadInfo.Value.Content.CopyToAsync(response.Body);

                return response;
            }

            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync($"File {filename} not found in {team} container.");
            return notFoundResponse;
        }
    }
}
