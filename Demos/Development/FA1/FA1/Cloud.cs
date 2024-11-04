/******************************************************************************
* Filename    = Cloud.cs
*
* Author      = Pranav Guruprasad Rao
*
* Product     = Cloud
* 
* Project     = Unnamed-Software-Engineering-Project
*
* Description = Cloud Abstraction Class
*****************************************************************************/

using System;
using System.IO;
using System.Drawing;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Cloud Abstraction Implementation
/// </summary>
public class Cloud : ICloud
{
    private readonly BlobService _blobService;

    /// <summary>
    /// Setup of the cloud
    /// <param name="containerName">Container Name</param>
    /// </summary>
    public Cloud(string containerName)
    {
        _blobService = new BlobService(containerName);
    }

    /// <summary>
    /// GET Implementation
    /// <param name="Sastoken">SAS Token used to access a particular Container</param>
    /// <param name="containerName">Container name to access</param>
    /// <param name="dataUri">Path parameter representing the filename to access.</param>
    /// <returns>JSON File containing the data object and its name</returns>
    /// </summary>
    public async Task<Dictionary<string, object>> Get(string SasToken, string containerName, string dataUri)
    {
        try
        {
            FileDownload fileDownload = new FileDownload();

            /// yet to fill in the HTTP request and execution context details here
            HttpResponseData response = fileDownload.DownloadFile(containerName, dataUri);

            if(response.StatusCode == HttpStatusCode.InternalServerError
                || response.StatusCode == HttpStatusCode.NotFound) {
                throw;
            }
            /// parse the HttpResponseData into JSON file
            jsonFile = null;

            /// convert JSON File into Dictionary
            return JsonSerializer.Deserialize<Dictionary<string, object>>(jsonFile);
        }
        catch (Exception ex)
        {
            // Handle and log the exception
            Console.WriteLine($"Error in Get: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// POST Implementation
    /// <param name="Sastoken">SAS Token used to access a particular Container</param>
    /// <param name="containerName">Container name to access</param>
    /// <param name="dataUri">Path parameter representing the filename to post.</param>
    /// <param name="data">Path parameter representing the data to post.</param>
    /// <returns>JSON File containing the new dataURI</returns>
    /// </summary>
    public async Task<Dictionary<string, string>> Post(string SasToken, string containerName, string dataUri, object data)
    {
        try
        {
            FileUpload fileUpload = new FileUpload();

            /// process the data object into a HTTP POST Request
            HttpResponseData response = fileUpload.UploadFile(containerName);

            if(response.StatusCode == HttpStatusCode.BadRequest
                || response.StatusCode == HttpStatusCode.BadRequest
                || response.StatusCode == HttpStatusCode.InternalServerError) {
                throw;
            }

            /// parse the HttpResponseData into JSON file
            jsonFile = null;

            /// convert JSON File into Dictionary
            return JsonSerializer.Deserialize<Dictionary<string, object>>(jsonFile);
        }
        catch (Exception ex)
        {
            /// Handle and log the exception
            Console.WriteLine($"Error in Post: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// PUT Implementation
    /// <param name="Sastoken">SAS Token used to access a particular Container</param>
    /// <param name="containerName">Container name to access</param>
    /// <param name="dataUri">Path parameter representing the filename to put.</param>
    /// <param name="data">Path parameter representing the data to put.</param>
    /// <returns>JSON File containing the bool value if updated</returns>
    /// </summary>
    public async Task<Dictionary<string, bool>> Put(string SasToken, string containerName, string oldDataUri, object data)
    {
        try
        {

        }
        catch (Exception ex)
        {
            // Handle and log the exception
            Console.WriteLine($"Error in Put: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// DELETE Implementation
    /// <param name="Sastoken">SAS Token used to access a particular Container</param>
    /// <param name="containerName">Container name to access</param>
    /// <param name="dataUri">Path parameter representing the filename to delete.</param>
    /// <returns>JSON File containing the bool value if deleted</returns>
    /// </summary>
    public async Task<Dictionary<string, bool>> Delete(string SasToken, string containerName, string dataUri)
    {
        try
        {
            FileDelete fileDelete = new FileDelete();
            HttpResponseData response = fileDelete.DeleteFile(containerName, dataUri);

            if(response.StatusCode == HttpStatusCode.InternalServerError
                || response.StatusCode == HttpStatusCode.NotFound
                || response.StatusCode == HttpStatusCode.BadRequest) {
                throw;
            }

            /// parse the HttpResponseData into JSON file
            jsonFile = null;

            /// convert JSON File into Dictionary
            return JsonSerializer.Deserialize<Dictionary<string, bool>>(jsonFile);
        }
        catch (Exception ex)
        {
            // Handle and log the exception
            Console.WriteLine($"Error in Delete: {ex.Message}");
            return false;
        }
    }
}