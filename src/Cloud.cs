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

    Dictionary<string, object> get(Dictionary<string, string> user_details, string folder, string data_uri)
    {

    }

    string put(Dictionary<string, string> user_details, string folder, object data)
    {

    }

    bool post(Dictionary<string, string> user_details, string folder, string old_data_uri, object data)
    {

    }

    bool delete(Dictionary<string, string> user_details, string folder, string data_uri)
    {

    }
}