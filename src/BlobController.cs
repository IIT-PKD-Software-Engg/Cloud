/******************************************************************************
* Filename    = BlobController.cs
*
* Author      = Pranav Guruprasad Rao
*
* Product     = Unnamed-Software-Engineering-Project
* 
* Project     = Cloud
*
* Description = Storage Controller API Class
*****************************************************************************/

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class BlobController : ControllerBase
{
	private readonly BlobService _blobService;

	public BlobController(BlobService blobService)
    {
    	_blobService = blobService;
    }

    [HttpGet]

    [HttpPost]

    [HttpPut]

    [HttpDelete]
}