/******************************************************************************
* Filename    = ICloud.cs
*
* Author      = Pranav Guruprasad Rao
*
* Product     = Cloud
* 
* Project     = Unnamed-Software-Engineering-Project
*
* Description = Cloud Interface
*****************************************************************************/

using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

/// <summary>
/// Interface for Cloud
/// </summary>
public interface ICloud {
	/// <summary>
    /// File Download function
    /// </summary>
    /// <param name="SasToken">Access Token for your container.</param>
    /// <param name="containerName">your particular container</param>
    /// <param name="dataUri">Path parameter representing the filename to access.</param>
    /// <returns>JSON File containing the data object and its name</returns>
	Dictionary<string, object> Get(string SasToken, string containerName, string dataUri);

    /// <summary>
    /// File Upload function
    /// </summary>
    /// <param name="SasToken">Access Token for your container.</param>
    /// <param name="containerName">The specific container to upload to</param>
    /// <param name="dataUri">Path parameter where the file will be uploaded</param>
    /// <param name="data">Object containing the data to be uploaded</param>
    /// <returns>JSON File containing the new dataURI of the file</returns>
	Dictionary<string, string> Post(string SasToken, string containerName, string dataUri, object data);

    /// <summary>
    /// File Update function
    /// </summary>
    /// <param name="SasToken">Access Token for your container.</param>
    /// <param name="containerName">The specific container to update in</param>
    /// <param name="oldDataUri">Path parameter of the existing file to be updated</param>
    /// <param name="data">New data to be updated in the file</param>
    /// <returns>JSON File containing Boolean indicating the success or failure of the update operation</returns>
	Dictionary<string, bool> Put(string SasToken, string containerName, string oldDataUri, object data);

    /// <summary>
    /// File Delete function
    /// </summary>
    /// <param name="SasToken">Access Token for your container.</param>
    /// <param name="containerName">The specific container where the file resides</param>
    /// <param name="dataUri">Path parameter representing the filename to delete</param>
    /// <returns>JSON File containing Boolean indicating the success or failure of the delete operation</returns>
	Dictionary<string, bool> Delete(string SasToken, string containerName, string dataUri);
}
