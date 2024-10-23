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
public class FileUploadTests
{
    private Mock<FunctionContext> _mockContext;
    private Mock<HttpRequestData> _mockRequest;
    private Mock<BlobContainerClient> _mockContainerClient;
    private Mock<BlobClient> _mockBlobClient;
    private HttpHeadersCollection _mockHeaders;

    [SetUp]
    public void SetUp()
    {
        // Initialize mocks for function context and HTTP request
        _mockContext = new Mock<FunctionContext>();
        _mockRequest = new Mock<HttpRequestData>(_mockContext.Object);
        _mockContainerClient = new Mock<BlobContainerClient>();
        _mockBlobClient = new Mock<BlobClient>();
        _mockHeaders = new HttpHeadersCollection();

        // Mock content-type for multipart form-data
        _mockHeaders.Add("Content-Type", "multipart/form-data; boundary=someBoundary");
        _mockRequest.Setup(r => r.Headers).Returns(_mockHeaders);
        _mockRequest.Setup(r => r.Body).Returns(new MemoryStream()); // Empty body
    }

    [Test]
    public async Task UploadFile_InvalidContentType_ReturnsBadRequest()
    {
        // Set invalid content type
        _mockHeaders.Clear();
        _mockHeaders.Add("Content-Type", "text/plain");

        // Act
        var response = await FileUpload.UploadFile(_mockRequest.Object, "team1", _mockContext.Object);

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Test]
    public async Task UploadFile_ValidFile_UploadsSuccessfully()
    {
        // Mock the BlobContainerClient and BlobClient behavior
        _mockContainerClient
            .Setup(c => c.GetBlobClient(It.IsAny<string>()))
            .Returns(_mockBlobClient.Object);

        _mockBlobClient
            .Setup(b => b.UploadAsync(It.IsAny<Stream>(), true))
            .Returns(Task.CompletedTask);

        // Act
        var response = await FileUpload.UploadFile(_mockRequest.Object, "team1", _mockContext.Object);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [Test]
    public async Task UploadFile_EmptyFileName_ReturnsBadRequest()
    {
        // Mock a request with an empty filename in content disposition
        _mockRequest.Setup(r => r.Body).Returns(new MemoryStream());
        var response = await FileUpload.UploadFile(_mockRequest.Object, "team1", _mockContext.Object);

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Test]
    public async Task UploadFile_ExceptionDuringUpload_ReturnsServerError()
    {
        // Simulate an exception during blob upload
        _mockBlobClient
            .Setup(b => b.UploadAsync(It.IsAny<Stream>(), true))
            .ThrowsAsync(new Exception("Upload error"));

        var response = await FileUpload.UploadFile(_mockRequest.Object, "team1", _mockContext.Object);

        Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Test]
    public async Task UploadEmptyFile()
    {
        // Arrange: Simulate an empty file upload
        _mockRequest.Setup(req => req.Headers).Returns(_mockHeaders);
        _mockRequest.Setup(req => req.Body).Returns(new MemoryStream()); // Empty stream

        var responseMock = new Mock<HttpResponseData>(_mockFunctionContext.Object);
        _mockRequest.Setup(req => req.CreateResponse(It.IsAny<HttpStatusCode>()))
                    .Returns(responseMock.Object);

        // Act: Call the UploadFile method
        var response = await FileUpload.UploadFile(_mockRequest.Object, "team1", _mockFunctionContext.Object);

        // Assert: Ensure that the response is a BadRequest
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        _mockLogger.Verify(l => l.LogWarning(It.Is<string>(s => s.Contains("No valid file provided"))), Times.Once);
    }

    [Test]
    public async Task UploadImageFile()
    {
        // Arrange: Simulate an image file upload
        var fileContent = Encoding.UTF8.GetBytes("fake image data");
        var fileStream = new MemoryStream(fileContent);
        _mockRequest.Setup(req => req.Headers).Returns(_mockHeaders);
        _mockRequest.Setup(req => req.Body).Returns(fileStream);

        var responseMock = new Mock<HttpResponseData>(_mockFunctionContext.Object);
        _mockRequest.Setup(req => req.CreateResponse(It.IsAny<HttpStatusCode>()))
                    .Returns(responseMock.Object);

        // Act: Call the UploadFile method
        var response = await FileUpload.UploadFile(_mockRequest.Object, "team1", _mockFunctionContext.Object);

        // Assert: Ensure the response is Ok
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        _mockLogger.Verify(l => l.LogInformation(It.Is<string>(s => s.Contains("uploaded successfully"))), Times.Once);
    }

    [Test]
    public async Task UploadLargeFile()
    {
        // Arrange: Simulate a large file upload (1GB for the test)
        var largeFileContent = new byte[1024 * 1024 * 1024]; // 1GB file
        new Random().NextBytes(largeFileContent); // Randomize content
        var largeFileStream = new MemoryStream(largeFileContent);

        _mockRequest.Setup(req => req.Headers).Returns(_mockHeaders);
        _mockRequest.Setup(req => req.Body).Returns(largeFileStream);

        var responseMock = new Mock<HttpResponseData>(_mockFunctionContext.Object);
        _mockRequest.Setup(req => req.CreateResponse(It.IsAny<HttpStatusCode>()))
                    .Returns(responseMock.Object);

        // Act: Call the UploadFile method
        var response = await FileUpload.UploadFile(_mockRequest.Object, "team1", _mockFunctionContext.Object);

        // Assert: Ensure the response is Ok
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        _mockLogger.Verify(l => l.LogInformation(It.Is<string>(s => s.Contains("uploaded successfully"))), Times.Once);
    }

    [Test]
    public async Task InvalidFileSection()
    {
        // Arrange: Simulate invalid file section in multipart form data
        _mockRequest.Setup(req => req.Headers).Returns(_mockHeaders);

        // Invalid body (not a valid file section)
        var invalidContent = Encoding.UTF8.GetBytes("Invalid multipart form data");
        var invalidStream = new MemoryStream(invalidContent);
        _mockRequest.Setup(req => req.Body).Returns(invalidStream);

        var responseMock = new Mock<HttpResponseData>(_mockFunctionContext.Object);
        _mockRequest.Setup(req => req.CreateResponse(It.IsAny<HttpStatusCode>()))
                    .Returns(responseMock.Object);

        // Act: Call the UploadFile method
        var response = await FileUpload.UploadFile(_mockRequest.Object, "team1", _mockFunctionContext.Object);

        // Assert: Ensure the response is a BadRequest
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        _mockLogger.Verify(l => l.LogWarning(It.Is<string>(s => s.Contains("Invalid content type"))), Times.Once);
    }
}
