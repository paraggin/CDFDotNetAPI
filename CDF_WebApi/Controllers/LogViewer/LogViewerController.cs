using CDF_Services.IServices.ILogViewerService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace CDF_WebApi.Controllers.LogViewer
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class LogViewerController : ControllerBase
    { 

        private readonly ILogViewerService _logviewerService;

    public LogViewerController(ILogViewerService logviewerService)
    {
            _logviewerService = logviewerService;
    }

    [HttpGet]
    [Route("GetAllStorageAccounts")]
    public async Task<IActionResult> GetAllStorageAccounts()
    {

        return await _logviewerService.GetAllStorageAccounts();

    }


    [HttpGet]
    [Route("GetStorageContainer")]
    public async Task<IActionResult> GetStorageContainer(string storageAccountName)
    {

        return await _logviewerService.GetAllAccountContainer(storageAccountName);

    }

    [HttpGet]
    [Route("GetContainerBlob")]
    public async Task<IActionResult> GetContainerBlob(string storageAccountName, string containerName)
    {

        return await _logviewerService.GetAllContainerBlob(storageAccountName, containerName);

    }

    [HttpGet]
    [Route("FilterContainerBlob")]
    public async Task<IActionResult> FilterContainerBlob(string storageAccountName, string containerName, string searchText)
    {

        return await _logviewerService.FilterContainerBlob(storageAccountName, containerName, searchText);

    }

    [HttpGet]
    [Route("GetBlobSasUrl")]
    public async Task<IActionResult> GetBlobSasUrl(string storageAccountName, string containerName, string blobName)
    {

        return await _logviewerService.GetBlobSasUrl(storageAccountName, containerName, blobName);

    }


    [HttpGet]
    [Route("downloadBlob")]
    public async Task<IActionResult> downloadBlob(string storageAccountName, string containerName, string blobName)
    {

        return await _logviewerService.downloadBlob(storageAccountName, containerName, blobName);

    }   
        }
}
