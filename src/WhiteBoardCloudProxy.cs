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
	string folder = 'whiteboard';
	Dictionary<string, string> user_auth;

	WhiteBoardCloudProxy(Dictionary<string, string> user_details) {
		cloudService = new Cloud(user_details);
		user_auth = user_details;
	}

	Dictionary<string,object> get(string data_uri)
	{
		return cloudService.get(user_auth, folder, data_uri);
	}

	data_uri put(string folder, object data)
	{
		return cloudService.data_uri put(user_auth, folder, data)
	}

	bool post(string folder, string old_data_uri, object data)
	{
		return cloudService.post(user_auth, folder, old_data_uri, data)
	}

	bool delete(string folder, string data_uri)
	{
		return cloudService.delete(user_auth, folder, data_uri)
	}
}