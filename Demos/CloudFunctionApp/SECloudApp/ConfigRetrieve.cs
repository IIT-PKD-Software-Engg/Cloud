using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace SECloudApp
{
    public class ConfigRetrieve
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<ConfigRetrieve> _logger;
        private readonly IConfiguration _configuration;

        public ConfigRetrieve(BlobServiceClient blobServiceClient, ILogger<ConfigRetrieve> logger, IConfiguration configuration)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
            _configuration = configuration;
        }

        [Function("GetConfigSetting")]
        public async Task<HttpResponseData> GetConfigSetting(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "config/{team}/{setting}")] HttpRequestData req,
            string team,
            string setting)
        {
            _logger.LogInformation($"Fetching configuration setting: {setting}.");

            string connectionString = _configuration["AzureWebJobsStorage"];
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(team);
            BlobClient configBlobClient = containerClient.GetBlobClient("config_filename.json");

            if (await configBlobClient.ExistsAsync())
            {
                var configContent = await configBlobClient.DownloadContentAsync();
                var configJson = configContent.Value.Content.ToString();
                using (JsonDocument doc = JsonDocument.Parse(configJson))
                {
                    if (doc.RootElement.TryGetProperty(setting, out JsonElement configValue))
                    {
                        var response = req.CreateResponse(HttpStatusCode.OK);
                        await response.WriteAsJsonAsync(new
                        {
                            setting = setting,
                            value = configValue.ToString()
                        });
                        return response;
                    }
                }
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                await notFoundResponse.WriteStringAsync($"Setting {setting} not found.");
                return notFoundResponse;
            }

            return req.CreateResponse(HttpStatusCode.NotFound);
        }
    }
}
