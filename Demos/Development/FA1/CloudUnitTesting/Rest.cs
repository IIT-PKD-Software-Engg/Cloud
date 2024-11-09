// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Net.Http;
using System.Reflection.Metadata;

namespace CloudUnitTesting;
internal class Rest
{
    private HttpClient _entityClient;
    private string _url;

    public Rest()
    {
        _entityClient = new();
        _url = "https://secloudapp-2024.azurewebsites.net/api/";
    }

    //public async void FileUploadTest()
    //{
    //   HttpResponseMessage response = await _entityClient.PostAsync(_url + $"/upload/testblobcontainer/peer.cu",
    //     );
    //response.EnsureSuccessStatusCode();

    //   var result = await response.Content.ReadAsStringAsync();
    //  Assert.Pass();
    //}

    public async Task<Entity?> FileDownloadAsyncTest(string container, string data)
    {
        HttpResponseMessage response = await _entityClient.GetAsync(_url + $"/download/{container}/{data}");
        response.EnsureSuccessStatusCode();

        string result = await response.Content.ReadAsStringAsync();

        Entity? entity = JsonSerializer.Deserialize<Entity>(result);
        return entity;
    }

    public async Task<Entity?> ConfigFileRetrieveAsyncTest(string container, string data)
    {
        HttpResponseMessage response = await _entityClient.GetAsync(_url + $"/config/{container}/{data}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions {
            ProprtyNameCaseInsensitive = true,
        };

        Entity? entity = JsonSerializer.Deserialize<Entity>(result, options);
        return entity;
    }

    public async Task<Entity?> FileDeleteAsyncTest(string container, string data)
    {
        HttpResponseMessage response = await _entityClient.DeleteAsync(_url + $"/delete/{container}/{data}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions {
            ProprtyNameCaseInsensitive = true,
        };

        Entity? entity = JsonSerializer.Deserialize<Entity>(result, options);
        return entity;
    }

    //public async Task<Entity?> FileUpdateAsyncTest(string container, string dataUri, string data)
    //{
    //    HttpResponseMessage response = await _entityClient.DeleteAsync(_url + $"/download/testblobcontainer/peer.cu");
    //    response.EnsureSuccessStatusCode();
    //
     //   var result = await response.Content.ReadAsStringAsync();
      //  var options = new JsonSerializerOptions {
        //    ProprtyNameCaseInsensitive = true,
      //  };

      //  Entity? entity = JsonSerializer.Deserialize<Entity>(result, options);
      //  return entity;
    //}
}
