using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace FA1
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
            try
            {
                _logger.LogInformation($"Function triggered. Team: {team}, Setting: {setting}");

                // Log connection string existence (don't log the actual string!)
                string connectionString = _configuration["AzureWebJobsStorage"];
                _logger.LogInformation($"Connection string exists: {!string.IsNullOrEmpty(connectionString)}");

                // Log container client creation attempt
                _logger.LogInformation($"Attempting to get container client for team: {team}");
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(team);

                // Log blob client creation attempt
                _logger.LogInformation("Attempting to get blob client for configFile.json");
                BlobClient configBlobClient = containerClient.GetBlobClient("configFile.json");

                // Log blob existence check
                _logger.LogInformation("Checking if config blob exists...");
                var blobExists = await configBlobClient.ExistsAsync();
                _logger.LogInformation($"Config blob exists: {blobExists.Value}");

                if (blobExists.Value)
                {
                    _logger.LogInformation("Downloading blob content...");
                    var configContent = await configBlobClient.DownloadContentAsync();
                    var configJson = configContent.Value.Content.ToString();

                    _logger.LogInformation($"Downloaded JSON content length: {configJson.Length}");
                    _logger.LogInformation($"JSON Content: {configJson}"); // Be careful with this in production!

                    using (JsonDocument doc = JsonDocument.Parse(configJson))
                    {
                        _logger.LogInformation($"Attempting to find setting: {setting}");
                        if (doc.RootElement.TryGetProperty(setting, out JsonElement configValue))
                        {
                            _logger.LogInformation($"Setting found. Value: {configValue}");
                            var response = req.CreateResponse(HttpStatusCode.OK);
                            await response.WriteAsJsonAsync(new
                            {
                                setting = setting,
                                value = configValue.ToString()
                            });
                            return response;
                        }
                        else
                        {
                            _logger.LogWarning($"Setting '{setting}' not found in config file");
                            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                            await notFoundResponse.WriteStringAsync($"Setting {setting} not found in configuration.");
                            return notFoundResponse;
                        }
                    }
                }
                else
                {
                    _logger.LogWarning($"Config file not found for team: {team}");
                    var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                    await notFoundResponse.WriteStringAsync($"Configuration file not found for team: {team}");
                    return notFoundResponse;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing request. Team: {team}, Setting: {setting}");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error processing request: {ex.Message}");
                return errorResponse;
            }
        }
    }
}