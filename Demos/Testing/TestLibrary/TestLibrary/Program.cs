using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SECloud.Services;
using SECloud.Models;

namespace SECloudClient
{
    class UpdateBlobProgram
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
            var cloudService = new CloudService("https://secloudapp-2024.azurewebsites.net/api", "testblobcontainer", "sp=racwdli&st=2024-11-09T18:04:23Z&se=2024-11-10T02:04:23Z&spr=https&sv=2022-11-02&sr=c&sig=7sWxFG6gYREdnJSgHQ4GcKfAM1fzTViFGNag3rmK%2Fe8%3D", httpClient, logger);

            string blobName = "cloud_local_trial.txt";
            string contentType = "text/plain";

            // Open file as stream to use as content
            using var contentStream = new FileStream(@"C:\Users\ARNAV\Desktop\cloud_local_trial.txt", FileMode.Open, FileAccess.Read);

            var response = await cloudService.UpdateAsync(blobName, contentStream, contentType);

            if (response.Success)
            {
                logger.LogInformation("Blob updated successfully: {Message}", response.Message);
            }
            else
            {
                logger.LogError("Updating blob failed: {Message}", response.Message);
            }
        }
    }
}
