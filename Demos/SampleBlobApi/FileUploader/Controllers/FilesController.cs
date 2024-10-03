using FileUploader.Services;
using Microsoft.AspNetCore.Mvc;

namespace SampleBlobApi.Controllers;

[ApiController]
[Route("[controller]")]
public class FilesController : ControllerBase
{
    private readonly FileServices _fileServices;

    public FilesController(FileServices fileServices)
    {
        _fileServices = fileServices;
    }

    [HttpGet]
    public async Task<IActionResult> ListAllBlobs()
    {
        var result = await _fileServices.ListAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var result = await _fileServices.UploadAsync(file);
        return Ok(result);
    }

    [HttpGet]
    [Route("filename")]
    public async Task<IActionResult> Download(string filename)
    {
        var result = await _fileServices.DownloadAsync(filename);
        return File(result.Content, result.ContentType, result.Name);
    }

    [HttpDelete]
    [Route("filename")]
    public async Task<IActionResult> Delete(string filenamme)
    {
        var result = await _fileServices.DeleteAsync(filenamme);
        return Ok(result);
    }
}
