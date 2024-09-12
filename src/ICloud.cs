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
	Dictionary<string, object> Get(userDetails, string folder, string dataUri);
	int Put(userDetails, string folder, object data);
	bool Put(userDetails, string folder, string oldDataUri, object data);
	bool Delete(userDetails, string folder, string dataUri);
}