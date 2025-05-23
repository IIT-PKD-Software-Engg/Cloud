using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SECloud.Services;
using SECloud.Models;

namespace SECloudClient
{
    class SearchJsonProgram
    {
        static async Task Main(string[] args)
        {
            // Setup dependency injection and logging
            var serviceProvider = new ServiceCollection()
                .AddLogging(config => config.AddConsole().SetMinimumLevel(LogLevel.Debug))
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILogger<CloudService>>();
            var httpClient = new HttpClient();

            // Initialize CloudService with base URL, team/container, and optional SAS token
            var cloudService = new CloudService(
                "https://secloudapp-2024.azurewebsites.net/api",
                "testblobcontainer",
                "",
                httpClient,
                logger
            );

            try
            {
                // Search parameters
                string searchKey = "Theme";  // Example key to search for
                string searchValue = "True";  // Example value to search for

                logger.LogInformation("Starting JSON search for key: {Key}, value: {Value}", searchKey, searchValue);

                var searchResponse = await cloudService.SearchJsonFilesAsync(searchKey, searchValue);

                if (searchResponse.Success)
                {
                    logger.LogInformation("Search completed successfully");
                    logger.LogInformation("Found {Count} matches", searchResponse.Data.MatchCount);

                    // Process and display each match
                    foreach (var match in searchResponse.Data.Matches)
                    {
                        logger.LogInformation("Match found in file: {FileName}", match.FileName);

                        // Pretty print the JSON content
                        var jsonFormatted = JsonSerializer.Serialize(match.Content, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });
                        logger.LogInformation("File content:\n{Content}", jsonFormatted);
                    }
                }
                else
                {
                    logger.LogError("Search failed: {Message}", searchResponse.Message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while performing the search");
            }

            // Optional: Wait for user input before closing
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}