using CDF_Services.IServices.IDocViewerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CDF_WebApi.Controllers.DocViewer
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class DocViewerController : ControllerBase
    {

        private readonly IDocViewerService _docViewerService;

        public DocViewerController(IDocViewerService docViewerService)
        {
            _docViewerService = docViewerService;
        } 
        [HttpGet]
        [Route("GetContainerBlob")]
        public async Task<IActionResult> GetContainerBlob(string storageAccountName, string containerName)
        {

            return await _docViewerService.GetAllContainerBlob(storageAccountName, containerName);

        }

        [HttpGet]
        [Route("FilterContainerBlob")]
        public async Task<IActionResult> FilterContainerBlob(string storageAccountName, string containerName, string searchText)
        {

            return await _docViewerService.FilterContainerBlob(storageAccountName, containerName, searchText);

        }

        [HttpGet]
        [Route("GetBlobSasUrl")]
        public async Task<IActionResult> GetBlobSasUrl(string storageAccountName, string containerName, string blobName)
        {

            return await _docViewerService.GetBlobSasUrl(storageAccountName, containerName, blobName);

        }


        [HttpGet]
        [Route("downloadBlob")]
        public async Task<IActionResult> downloadBlob(string storageAccountName, string containerName, string blobName)
        {

            return await _docViewerService.downloadBlob(storageAccountName, containerName, blobName);

        }

    }
}
