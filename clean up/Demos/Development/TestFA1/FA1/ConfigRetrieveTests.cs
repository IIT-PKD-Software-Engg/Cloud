using NUnit.Framework;
using Moq;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using FA1;

[TestFixture]
public class ConfigRetrieveTests
{
    private Mock<BlobServiceClient> _mockBlobServiceClient;
    private Mock<BlobContainerClient> _mockContainerClient;
    private Mock<BlobClient> _mockBlobClient;
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<ILogger<ConfigRetrieve>> _mockLogger;
    private Mock<FunctionContext> _mockFunctionContext;
    private Mock<HttpRequestData> _mockRequest;

    private ConfigRetrieve _configRetrieve;

    [SetUp]
    public void SetUp()
    {
        // Initialize the mocks
        _mockBlobServiceClient = new Mock<BlobServiceClient>();
        _mockContainerClient = new Mock<BlobContainerClient>();
        _mockBlobClient = new Mock<BlobClient>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<ConfigRetrieve>>();
        _mockFunctionContext = new Mock<FunctionContext>();
        _mockRequest = new Mock<HttpRequestData>(_mockFunctionContext.Object);

        // Mock configuration retrieval for connection string
        _mockConfiguration.Setup(config => config["AzureWebJobsStorage"])
                          .Returns("UseDevelopmentStorage=true");

        // Mock blob container client and blob client creation
        _mockBlobServiceClient.Setup(client => client.GetBlobContainerClient(It.IsAny<string>()))
                              .Returns(_mockContainerClient.Object);
        _mockContainerClient.Setup(container => container.GetBlobClient(It.IsAny<string>()))
                            .Returns(_mockBlobClient.Object);

        // Initialize the ConfigRetrieve function class with mocked dependencies
        _configRetrieve = new ConfigRetrieve(_mockBlobServiceClient.Object, _mockLogger.Object, _mockConfiguration.Object);
    }

    [Test]
    public async Task GetConfigSetting_ConfigFileExists_ReturnsSettingValue()
    {
        // Arrange: Setup the blob client to simulate an existing config file with the expected setting
        string testSetting = "testSetting";
        string configJson = "{\"testSetting\":\"testValue\"}";
        var content = BinaryData.FromString(configJson);

        _mockBlobClient.Setup(blob => blob.ExistsAsync())
                       .ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));
        _mockBlobClient.Setup(blob => blob.DownloadContentAsync())
                       .ReturnsAsync(Response.FromValue(BlobsModelFactory.BlobDownloadResult(content), Mock.Of<Response>()));

        // Mock HTTP request
        _mockRequest.Setup(req => req.CreateResponse(It.IsAny<HttpStatusCode>()))
                    .Returns(new Mock<HttpResponseData>(_mockFunctionContext.Object).Object);

        // Act: Call the function to retrieve the setting
        var response = await _configRetrieve.GetConfigSetting(_mockRequest.Object, "team1", testSetting);

        // Assert: Ensure the response contains the correct setting value
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        _mockLogger.Verify(l => l.LogInformation(It.Is<string>(s => s.Contains("Setting found"))), Times.Once);
    }

    [Test]
    public async Task GetConfigSetting_ConfigFileDoesNotExist_ReturnsNotFound()
    {
        // Arrange: Setup the blob client to simulate a non-existent config file
        _mockBlobClient.Setup(blob => blob.ExistsAsync())
                       .ReturnsAsync(Response.FromValue(false, Mock.Of<Response>()));

        // Mock HTTP request
        _mockRequest.Setup(req => req.CreateResponse(It.IsAny<HttpStatusCode>()))
                    .Returns(new Mock<HttpResponseData>(_mockFunctionContext.Object).Object);

        // Act: Call the function to retrieve the setting
        var response = await _configRetrieve.GetConfigSetting(_mockRequest.Object, "team1", "testSetting");

        // Assert: Ensure the response is a 404 not found
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        _mockLogger.Verify(l => l.LogWarning(It.Is<string>(s => s.Contains("Config file not found"))), Times.Once);
    }

    [Test]
    public async Task GetConfigSetting_SettingNotFoundInConfig_ReturnsNotFound()
    {
        // Arrange: Setup the blob client to simulate an existing config file but the requested setting does not exist
        string configJson = "{\"someOtherSetting\":\"someValue\"}";
        var content = BinaryData.FromString(configJson);

        _mockBlobClient.Setup(blob => blob.ExistsAsync())
                       .ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));
        _mockBlobClient.Setup(blob => blob.DownloadContentAsync())
                       .ReturnsAsync(Response.FromValue(BlobsModelFactory.BlobDownloadResult(content), Mock.Of<Response>()));

        // Mock HTTP request
        _mockRequest.Setup(req => req.CreateResponse(It.IsAny<HttpStatusCode>()))
                    .Returns(new Mock<HttpResponseData>(_mockFunctionContext.Object).Object);

        // Act: Call the function to retrieve the setting
        var response = await _configRetrieve.GetConfigSetting(_mockRequest.Object, "team1", "nonexistentSetting");

        // Assert: Ensure the response is a 404 not found
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        _mockLogger.Verify(l => l.LogWarning(It.Is<string>(s => s.Contains("Setting 'nonexistentSetting' not found"))), Times.Once);
    }

    [Test]
    public async Task GetConfigSetting_ExceptionThrown_ReturnsServerError()
    {
        // Arrange: Simulate an exception during blob client operations
        _mockBlobClient.Setup(blob => blob.ExistsAsync())
                       .ThrowsAsync(new Exception("Blob storage error"));

        // Mock HTTP request
        _mockRequest.Setup(req => req.CreateResponse(It.IsAny<HttpStatusCode>()))
                    .Returns(new Mock<HttpResponseData>(_mockFunctionContext.Object).Object);

        // Act: Call the function and expect it to handle the exception
        var response = await _configRetrieve.GetConfigSetting(_mockRequest.Object, "team1", "testSetting");

        // Assert: Ensure the response is a 500 internal server error
        Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        _mockLogger.Verify(l => l.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
    }
}