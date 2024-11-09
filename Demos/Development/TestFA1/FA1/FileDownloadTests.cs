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
public class FileDownloadTests
{
    private Mock<FunctionContext> _mockContext;
    private Mock<HttpRequestData> _mockRequest;
    private Mock<BlobContainerClient> _mockContainerClient;
    private Mock<BlobClient> _mockBlobClient;

    [SetUp]
    public void SetUp()
    {
        _mockContext = new Mock<FunctionContext>();
        _mockRequest = new Mock<HttpRequestData>(_mockContext.Object);
        _mockContainerClient = new Mock<BlobContainerClient>();
        _mockBlobClient = new Mock<BlobClient>();
    }

    [Test]
    public async Task DownloadFile_FileExists_ReturnsOkWithFile()
    {
        // Mock the BlobClient behavior to simulate file existence
        _mockBlobClient
            .Setup(b => b.ExistsAsync())
            .ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));

        _mockBlobClient
            .Setup(b => b.DownloadAsync())
            .ReturnsAsync(Response.FromValue(
                BlobsModelFactory.BlobDownloadInfo(new MemoryStream(), BlobHttpHeaders.None, new BlobProperties(), null, null),
                Mock.Of<Response>()));

        // Act
        var response = await FileDownload.DownloadFile(_mockRequest.Object, "team1", "file1.txt", _mockContext.Object);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("application/octet-stream", response.Headers.GetValues("Content-Type").First());
    }

    [Test]
    public async Task DownloadFile_FileDoesNotExist_ReturnsNotFound()
    {
        // Simulate blob not existing
        _mockBlobClient
            .Setup(b => b.ExistsAsync())
            .ReturnsAsync(Response.FromValue(false, Mock.Of<Response>()));

        // Act
        var response = await FileDownload.DownloadFile(_mockRequest.Object, "team1", "nonexistentfile.txt", _mockContext.Object);

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Test]
    public async Task DownloadFile_ExceptionDuringDownload_ReturnsServerError()
    {
        // Simulate an exception during blob download
        _mockBlobClient
            .Setup(b => b.DownloadAsync())
            .ThrowsAsync(new Exception("Download error"));

        var response = await FileDownload.DownloadFile(_mockRequest.Object, "team1", "file1.txt", _mockContext.Object);

        Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Test]
    public async Task EmptyConnectionString()
    {
        // Arrange: Simulate an empty connection string
        _mockConfiguration.Setup(config => config["AzureWebJobsStorage"]).Returns(string.Empty);

        // Mock HTTP request creation
        _mockRequest.Setup(req => req.CreateResponse(It.IsAny<HttpStatusCode>()))
                    .Returns(new Mock<HttpResponseData>(_mockFunctionContext.Object).Object);

        // Act: Call the DownloadFile method
        var response = await FileDownload.DownloadFile(_mockRequest.Object, "team1", "testFile.txt", _mockFunctionContext.Object);

        // Assert: Ensure the response is a 500 internal server error
        Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        _mockLogger.Verify(l => l.LogError(It.Is<string>(s => s.Contains("Connection string not found"))), Times.Once);
    }

    [Test]
    public async Task InvalidFileName()
    {
        // Arrange: Simulate valid connection string and blob existence, but invalid file name
        string connectionString = "UseDevelopmentStorage=true";
        _mockConfiguration.Setup(config => config["AzureWebJobsStorage"]).Returns(connectionString);

        // Setup the blob client to simulate a non-existing blob (invalid file name)
        _mockBlobClient.Setup(blob => blob.ExistsAsync())
                       .ReturnsAsync(Response.FromValue(false, Mock.Of<Response>()));

        // Mock HTTP request creation
        _mockRequest.Setup(req => req.CreateResponse(It.IsAny<HttpStatusCode>()))
                    .Returns(new Mock<HttpResponseData>(_mockFunctionContext.Object).Object);

        // Act: Call the DownloadFile method
        var response = await FileDownload.DownloadFile(_mockRequest.Object, "team1", "invalidFile.txt", _mockFunctionContext.Object);

        // Assert: Ensure the response is a 404 Not Found
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        _mockLogger.Verify(l => l.LogWarning(It.Is<string>(s => s.Contains("File 'invalidFile.txt' not found in the container"))), Times.Once);
    }
}
