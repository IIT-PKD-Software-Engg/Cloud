/******************************************************************************
* Filename    = ICloud.cs
*
* Author      = Pranav Guruprasad Rao
*
* Product     = Unnamed-Software-Engineering-Project
* 
* Project     = Cloud
*
* Description = Cloud Interface
*****************************************************************************/

/// Cloud.Post(SASToken, "content", "/broadcast/messages.txt", data);

using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

public interface ICloud {
	Dictionary<string, object> Get(string SasToken, string containerName, string dataUri);
	int Post(string SasToken, string containerName, stirng dataUri, object data);
	bool Put(string SasToken, string containerName, string oldDataUri, object data);
	bool Delete(string SasToken, string containerName, string dataUri);
}