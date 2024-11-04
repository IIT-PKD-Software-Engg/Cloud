/******************************************************************************
* Filename    = ContentCloudProxy.cs
*
* Author      = Pranav Guruprasad Rao
*
* Product     = Unnamed-Software-Engineering-Project
* 
* Project     = Cloud
*
* Description = Content Cloud Use Class
*****************************************************************************/

using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

/// <summary>
/// Proxy Class for Content Team
/// </summary>
public class ContentCloudProxy {
    Cloud cloudService;
    string containerName = "content";   /// specific container details
    string _sasToken = "";  /// SAS Token for authentication

    /// <summary>
    /// Set up cloud service
    /// </summary>
    public ContentCloudProxy() {
        cloudService = new Cloud(_sasToken, containerName);
        _sasToken = SasToken;
    }

    /// <summary>
    /// GET functionality from cloud
    /// <param name="dataUri">data URI to access</param>
    /// <returns>Dictionary containing dataURI and data</returns>
    /// </summary>
    Dictionary<string, object> Get(string dataUri)
    {
        return cloudService.Get(_sasToken, containerName, dataUri);
    }

    /// <summary>
    /// POST functionality from cloud
    /// <param name="dataUri">data URI to post</param>
    /// <param name="data">data to post</param>
    /// <returns>new datURI</returns>
    /// </summary>
    string Post(string dataUri, object data)
    {
        Dictionary<string, string> responseDict = cloudService.Post(_sasToken, containerName, dataUri, data);
        return responseDict[];
    }

    /// <summary>
    /// PUT functionality from cloud
    /// <param name="oldDataUri">data URI to update</param>
    /// <param name="data">data to update</param>
    /// <returns>bool whether it is updated</returns>
    /// </summary>
    bool Put(string oldDataUri, object data)
    {
        Dictionary<string, bool> responseDict = cloudService.Put(_sasToken, containerName, oldDataUri, data)
        return responseDict[];
    }

    /// <summary>
    /// DELETE functionality from cloud
    /// <param name="dataUri">data URI to delete</param>
    /// <returns>bool whether it is updated</returns>
    /// </summary>
    bool Delete(string dataUri)
    {
        Dictionary<string, bool> responseDict = cloudService.Delete(_sasToken, containerName, dataUri)
        return responseDict[];
    }
}
