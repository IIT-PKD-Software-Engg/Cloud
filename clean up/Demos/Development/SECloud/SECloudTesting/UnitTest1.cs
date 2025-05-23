using System.Text;
using System;
using NUnit.Framework;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Text.Json;
using System.Net.Http.Json;
using System.Net.Http;
using System.Reflection.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Services;
using Models;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace SECloudTesting
{
    public class Tests
    {
        private HttpClient _httpClient;
        private ILogger<CloudService> _logger;
        public string _team = "testblobcontainer";
        public string _sasToken = "";
        private const string BaseUrl = "https://secloudapp-2024.azurewebsites.net/api/";

        [SetUp]
        public void Setup()
        {
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddLogging(builder => {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Debug);
                })
                .AddHttpClient()
                .BuildServiceProvider();
            _logger = serviceProvider.GetRequiredService<ILogger<CloudService>>();
            _httpClient = new HttpClient();
        }

        /*
        [Test]
        public async Task ExceptionFileUploadTest()
        {
            _logger.LogInformation("Testing for Internal Server Error in File Upload.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            string content = "This is a test file content";
            string fileName = $"test-{Guid.NewGuid()}.txt";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            ServiceResponse<string> response = await cloudService.UploadAsync(fileName, stream, "text/plain");

            Assert.That(response.Success, Is.EqualTo(false));
        }

        [Test]
        public async Task InvalidContentTypeFileUploadTest()
        {
            _logger.LogInformation("Uploading File with Invalid Content Type.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            string content = "This is a test file content";
            string fileName = $"test-{Guid.NewGuid()}.txt";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            ServiceResponse<string> response = await cloudService.UploadAsync(fileName, stream, "text/plain");

            Assert.That(response.Success, Is.EqualTo(false));
        }

        [Test]
        public async void InvalidFileSectionFileUploadTest()
        {
            _logger.LogInformation("Uploading File with Invalid File Section.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            string content = "This is a test file content";
            string fileName = $"test-{Guid.NewGuid()}.txt";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            ServiceResponse<string> response = await cloudService.UploadAsync(fileName, stream, "text/plain");

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(response.Success, false);
        }*/

        [Test]
        public async Task LargeFileUploadTest()
        {
            _logger.LogInformation("Uploading Large File.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            int size = 5 * 1024 * 1024; // 5MB
            byte[] buffer = new byte[size];
            new Random().NextBytes(buffer);

            string fileName = $"test-{Guid.NewGuid()}.bin";
            //string filePath = "path/to/your/file.dat";
            using var stream = new MemoryStream(buffer);
            ServiceResponse<string> response = await cloudService.UploadAsync(fileName, stream, "application/octet-stream");

            Assert.That(response.Success, Is.EqualTo(true));
        }

        [Test]
        public async Task ImageFileUploadTest()
        {
            _logger.LogInformation("Uploading Image File.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            // Download the image from the provided URL
            string imageUrl = "https://picsum.photos/id/1/200/300";
            using var httpClient = new HttpClient();
            byte[] imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
            using var stream = new MemoryStream(imageBytes);
            // Upload the downloaded image
            ServiceResponse<string> response = await cloudService.UploadAsync("image.jpg", stream, "image/jpeg");
            Assert.That(response.Success, Is.EqualTo(true));
        }

        /*
        [Test]
        public async Task EmptyFileUploadTest()
        {
            _logger.LogInformation("Uploading Empty File.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            // string content = null;
            string fileName = $"test-{Guid.NewGuid()}.txt";
            File.CreateText(fileName).Close();
            string content = File.ReadAllText(fileName);
            using var stream = new MemoryStream();
            ServiceResponse<string> response = await cloudService.UploadAsync(fileName, stream, "text/plain");
            Assert.That(response.Success, Is.EqualTo(false));
        }

        [Test]
        public async Task EmptyFileNameUploadTest()
        {
            _logger.LogInformation("Uploading File with no file name.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            string content = "This is a test file content";
            string fileName = "";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            ServiceResponse<string> response = await cloudService.UploadAsync(fileName, stream, "text/plain");

            Assert.That(response.Success, Is.EqualTo(false));
        }*/

        [Test]
        public async Task SuccessfulFileUploadTest()
        {
            _logger.LogInformation("Uploading Regular File.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            string content = "This is a test file content";
            string fileName = $"test-1.txt";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            ServiceResponse<string> response = await cloudService.UploadAsync(fileName, stream, "text/plain");

            Assert.That(response.Success, Is.EqualTo(true));
        }

        [Test]
        public async Task ConcurrentFileUploadTest()
        {
            _logger.LogInformation("Uploading files concurrently.");
            _logger.LogInformation("Uploading Regular File.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            var tasks = new List<Task<ServiceResponse<string>>>();
            for (int i = 0; i < 5; i++)
            {
                string content = $"Concurrent test file {i}";
                string fileName = $"concurrent-test-{i}-{Guid.NewGuid()}.txt";
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

                tasks.Add(cloudService.UploadAsync(fileName, stream, "text/plain"));
            }
            ServiceResponse<string>[] results = await Task.WhenAll(tasks);

            for (int i = 0; i < results.Length; i++)
            {
                Assert.That(results[i].Success, Is.EqualTo(true));
            }

        }

        [Test]
        public async Task InvalidFileNameDownloadTest()
        {
            _logger.LogInformation("Downloading File with invalid file name.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            string blobName = $"{Guid.NewGuid()}";
            ServiceResponse<System.IO.Stream> downloadResponse = await cloudService.DownloadAsync(blobName);
            Assert.That(downloadResponse.Success, Is.EqualTo(false));
        }

        /*
        [Test]
        public async void EmptyConnectionStringFileDownloadTest()
        {
            _logger.LogInformation("Downloading File with empty connection string.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            string blobName = $"{Guid.NewGuid()}.txt";
            ServiceResponse<System.IO.Stream> downloadResponse = await cloudService.DownloadAsync(blobName);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(downloadResponse.Success, false);
        }

        [Test]
        public async void ExceptionFileDownloadTest()
        {
            _logger.LogInformation("Testing for Internal Server Error in File Download.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            string blobName = $"{Guid.NewGuid()}.txt";
            ServiceResponse<System.IO.Stream> downloadResponse = await cloudService.DownloadAsync(blobName);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(downloadResponse.Success, false);
        }*/

        [Test]
        public async Task NonExistentFileDownloadTest()
        {
            _logger.LogInformation("Downloading Non-existent File.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            string blobName = $"{Guid.NewGuid()}.txt";
            ServiceResponse<System.IO.Stream> downloadResponse = await cloudService.DownloadAsync(blobName);
            Assert.That(downloadResponse.Success, Is.EqualTo(false));
        }

        [Test]
        public async Task SuccessfulFileDownloadTest()
        {
            _logger.LogInformation("Downloading Regular File.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            string blobName = "test-1.txt";
            ServiceResponse<System.IO.Stream> downloadResponse = await cloudService.DownloadAsync(blobName);
            Assert.That(downloadResponse.Success, Is.EqualTo(true));
        }

        [Test]
        public async Task SuccessfulConfigFileRetrievalTest()
        {
            _logger.LogInformation("Getting Correct Config Setting.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            string setting = "Theme";
            ServiceResponse<string> response = await cloudService.RetrieveConfigAsync(setting);
            Assert.That(response.Success, Is.EqualTo(true));
        }

        /*
        [Test]
        public async void NonExistentConfigFileRetrievalTest()
        {
            _logger.LogInformation("Testing Config Retrieval with non existent config file.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            string setting = "Value";
            ServiceResponse<string> response = await cloudService.RetrieveConfigAsync(setting);
            Assert.That(response.Success, Is.EqualTo(false));
        }
        */

        [Test]
        public async Task NonExistentSettingConfigFileRetrievalTest()
        {
            _logger.LogInformation("Testing Config Retrieval with non existent config setting.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            string setting = $"{Guid.NewGuid()}";
            ServiceResponse<string> response = await cloudService.RetrieveConfigAsync(setting);
            Assert.That(response.Success, Is.EqualTo(false));
        }

        /*
        [Test]
        public async void ExceptionConfigFileRetrievalTest()
        {
            _logger.LogInformation("Testing for Internal Server Error in Config File Retrieval.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            string setting = "Theme";
            ServiceResponse<string> response = await cloudService.RetrieveConfigAsync(setting);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(response.Success, false);
        }*/

        [Test]
        public async Task SuccessfulFileUpdateTest()
        {
            _logger.LogInformation("Updating Regular File.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            string contentType = "text/plain";

            string content = "This is the updated version of the test file content";
            string fileName = $"test-1.txt";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            ServiceResponse<string> response = await cloudService.UpdateAsync(fileName, stream, contentType);
            Assert.That(response.Success, Is.EqualTo(true));
        }

        [Test]
        public async Task SuccessfulFileDeleteTest()
        {
            _logger.LogInformation("Deleting Regular File.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            string content = "TO BE DELETED";
            string fileName = $"test-2.txt";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            ServiceResponse<string> response = await cloudService.UploadAsync(fileName, stream, "text/plain");

            if (response.Success == true)
            {
                ServiceResponse<bool> deleteResponse = await cloudService.DeleteAsync(fileName);
                Assert.That(deleteResponse.Success, Is.EqualTo(true));
            }
        }

        [Test]
        public async Task InvalidFileNameDeleteTest()
        {
            _logger.LogInformation("Deleting Regular File.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            string blobName = $"{Guid.NewGuid()}";
            ServiceResponse<bool> response = await cloudService.DeleteAsync(blobName);
            Assert.That(response.Success, Is.EqualTo(false));
        }

        [Test]
        public async Task ListBlobsTest()
        {
            _logger.LogInformation("Updating Regular File.");
            var cloudService = new CloudService(
                BaseUrl,
                _team,
                _sasToken,
                _httpClient,
                _logger
            );

            ServiceResponse<System.Collections.Generic.List<string>> response = await cloudService.ListBlobsAsync();
            Assert.That(response.Success, Is.EqualTo(true));
        }
    }
}