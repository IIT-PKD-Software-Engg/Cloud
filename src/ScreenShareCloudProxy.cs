/******************************************************************************
* Filename    = ScreenshareProxy.cs
*
* Author      = Pranav Guruprasad Rao
*
* Product     = Unnamed-Software-Engineering-Project
* 
* Project     = Cloud
*
* Description = Screenshare Cloud Use Class
*****************************************************************************/

using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

public class ScreenshareCloudProxy {
	Cloud cloudService;
	string folder = "screenshare";
	Dictionary<string, string> userAuth;

	ScreenshareCloudProxy(Dictionary<string, string> userDetails) {
		cloudService = new Cloud(userDetails);
		userAuth = userDetails;
	}

	Dictionary<string,object> Get(string dataUri)
	{
		return cloudService.Get(userAuth, folder, dataUri);
	}

	string Post(string folder, object data)
	{
		return cloudService.dataUri Put(userAuth, folder, data)
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
