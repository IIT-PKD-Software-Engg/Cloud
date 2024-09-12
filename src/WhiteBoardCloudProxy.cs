/******************************************************************************
* Filename    = WhiteBoardProxy.cs
*
* Author      = Pranav Guruprasad Rao
*
* Product     = Unnamed-Software-Engineering-Project
* 
* Project     = Cloud
*
* Description = WhiteBoard Cloud Use Class
*****************************************************************************/

using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

public class WhiteBoardCloudProxy {
	Cloud cloudService;
	string folder = "whiteboard";
	Dictionary<string, string> userAuth;

	WhiteBoardCloudProxy(Dictionary<string, string> userDetails) {
		cloudService = new Cloud(userDetails);
		userAuth = userDetails;
	}

	Dictionary<string,object> Get(string dataUri)
	{
		return cloudService.Get(userAuth, folder, dataUri);
	}

	string Post(string folder, object data)
	{
		return cloudService.Post(userAuth, folder, data)
	}

	bool Put(string folder, string oldDataUri, object data)
	{
		return cloudService.Put(userAuth, folder, oldDataUri, data)
	}

	bool Delete(string folder, string dataUri)
	{
		return cloudService.Delete(userAuth, folder, dataUri)
	}
}
