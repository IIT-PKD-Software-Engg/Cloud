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

using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

public interface ICloud {
	Dictionary<string, object> get(user_details, string folder, string data_uri);
	int put(user_details, string folder, object data);
	bool post(user_details, string folder, string old_data_uri, object data);
	bool delete(user_details, string folder, string data_uri);
}