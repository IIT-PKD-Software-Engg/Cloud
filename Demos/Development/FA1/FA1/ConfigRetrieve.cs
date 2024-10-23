/******************************************************************************
* Filename    = ConfigRetrieve.cs
* Author      = Arnav Rajesh Kadu
* Product     = Cloud
* Project     = Unnamed Software Project
* Description = To process configuration requests
*****************************************************************************/

using Azure.Storage.Blobs; // Import Azure Blob Storage SDK for managing blob data.
using Microsoft.Azure.Functions.Worker.Http; // Import HTTP handling components for Azure Functions.
using Microsoft.Azure.Functions.Worker; // Import core Azure Functions components.
using Microsoft.Extensions.Configuration; // Import configuration support (e.g., app settings).
using Microsoft.Extensions.Logging; // Import logging functionality.
using System; // Basic System functionality (exceptions, etc.).
using System.Net; // For handling HTTP status codes.
using System.Text.Json; // For working with JSON data.
using System.Threading.Tasks; // Provides support for async/await.

namespace FA1 // Declare the namespace for the function.
{
    /// <summary>
    /// Configuration Retrieval Class
    /// </summary>
    public class ConfigRetrieve
    {
        // Private members to hold BlobServiceClient, logger, and configuration references.
        private readonly BlobServiceClient _blobServiceClient; 
        private readonly ILogger<ConfigRetrieve> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for ConfigRetrieve class
        /// Initializes BlobServiceClient, Logger, and Configuration instances.
        /// </summary>
        public ConfigRetrieve(BlobServiceClient blobServiceClient, ILogger<ConfigRetrieve> logger, IConfiguration configuration)
        {
            _blobServiceClient = blobServiceClient; // Assign the BlobServiceClient passed as a dependency.
            _logger = logger; // Assign the logger passed as a dependency.
            _configuration = configuration; // Assign the configuration (e.g., settings).
        }

        /// <summary>
        /// Azure Function to retrieve configuration settings from a blob file.
        /// </summary>
        /// <param name="req">HTTP request data.</param>
        /// <param name="team">Path parameter for the team name used as blob container name</param>
        /// <param name="setting">Path parameter for the specific setting to retrieve.</param>
        /// <returns>An HTTP response with the requested setting or error.</returns>
        [Function("GetConfigSetting")]
        public async Task<HttpResponseData> GetConfigSetting(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "config/{team}/{setting}")] HttpRequestData req,
            string team,
            string setting)
        {
            try
            {
                _logger.LogInformation($"Function triggered. Team: {team}, Setting: {setting}");

                string connectionString = _configuration["AzureWebJobsStorage"];
                // Log whether the connection string is available (but not the actual connection string).
                _logger.LogInformation($"Connection string exists: {!string.IsNullOrEmpty(connectionString)}");

                _logger.LogInformation($"Attempting to get container client for team: {team}");
                // Get the container client.
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(team);

                _logger.LogInformation("Attempting to get blob client for configFile.json");
                // Create a blob for the config file
                BlobClient configBlobClient = containerClient.GetBlobClient("configFile.json");

                _logger.LogInformation("Checking if config blob exists...");
                // Check if the blob file exists.
                var blobExists = await configBlobClient.ExistsAsync();
                _logger.LogInformation($"Config blob exists: {blobExists.Value}");

                if (blobExists.Value)
                {
                    _logger.LogInformation("Downloading blob content...");
                     // Download blob content and convert the content to string.
                    var configContent = await configBlobClient.DownloadContentAsync();
                    var configJson = configContent.Value.Content.ToString();

                    _logger.LogInformation($"Downloaded JSON content length: {configJson.Length}");
                    _logger.LogInformation($"JSON Content: {configJson}"); 

                    // Parse the JSON content into a JsonDocument for querying.
                    using (JsonDocument doc = JsonDocument.Parse(configJson))
                    {
                        _logger.LogInformation($"Attempting to find setting: {setting}");
                        if (doc.RootElement.TryGetProperty(setting, out JsonElement configValue)) // Check if the setting exists.
                        {
                            _logger.LogInformation($"Setting found. Value: {configValue}");
                            var response = req.CreateResponse(HttpStatusCode.OK); // Create a success response.
                            await response.WriteAsJsonAsync(new
                            {
                                setting = setting,
                                value = configValue.ToString()
                            });   // Return the setting value.
                            return response; // Return the response.
                        }
                        else
                        {
                            _logger.LogWarning($"Setting '{setting}' not found in config file");
                            // Create a not found response.
                            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                            await notFoundResponse.WriteStringAsync($"Setting {setting} not found in configuration.");
                            return notFoundResponse; // Return the not found response.
                        }
                    }
                }
                else // If the blob does not exist:
                {
                    _logger.LogWarning($"Config file not found for team: {team}");
                    // Create a not found response.
                    var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                    await notFoundResponse.WriteStringAsync($"Configuration file not found for team: {team}");
                    return notFoundResponse; // Return the not found response.
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing request. Team: {team}, Setting: {setting}");
                // Create an error response and return the error message.
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error processing request: {ex.Message}");
                return errorResponse; // Return the error response.
            }
        }
    }
}
