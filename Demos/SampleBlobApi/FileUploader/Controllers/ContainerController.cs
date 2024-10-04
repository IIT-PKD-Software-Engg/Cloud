using FileUploader.Services;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace FileUploader.Controllers;

[ApiController]
[Route("[controller]")]
public class ContainerController : ControllerBase
{
    private readonly ContainerServices _containerServices;
    public ContainerController(ContainerServices containerServices)
    {
        _containerServices = containerServices;
    }
    /*
    [HttpGet]
    public async Task<IActionResult> ListContainerDirectory(string containerName)
    {
        return Ok();
    }
    */
    [HttpGet]
    public async Task<IActionResult> ListAllContainers()
    {
        var result = await _containerServices.ListContainersAsync();
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> CreateContainer(string containerName)
    {
        var result = await _containerServices.CreateContainerAsync(containerName);
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteContainer(string containerName)
    {
        await _containerServices.DeleteContainerAsync(containerName);
        return Ok();
    }
}
