/******************************************************************************
* Filename    = Cloud.cs
*
* Author      = Pranav Guruprasad Rao
*
* Product     = Unnamed-Software-Engineering-Project
* 
* Project     = Cloud
*
* Description = Cloud Abstraction Class
*****************************************************************************/

using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

public class Cloud: ICloud {
    IStorage storage;
    IDatabase database;

    Dictionary<string, object> Get(Dictionary<string, string> userDetails, string folder, string dataUri)
    {

    }

    string Put(Dictionary<string, string> userDetails, string folder, object data)
    {

    }

    bool Put(Dictionary<string, string> userDetails, string folder, string oldDataUri, object data)
    {

    }

    bool Delete(Dictionary<string, string> userDetails, string folder, string dataUri)
    {

    }
}