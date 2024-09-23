using CDF_Services.IServices.IBlobStorageService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CDF_WebApi.Controllers.BolbStorage
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class BlobByStorageAcController : ControllerBase
    {

        private readonly IBlobByStorageAcService _blobByStorageAcService;

        public BlobByStorageAcController(IBlobByStorageAcService blobByStorageAcService)
        {
            _blobByStorageAcService = blobByStorageAcService;
        }

        [HttpGet]
        [Route("GetAllStorageAccounts")]
        public async Task<IActionResult> GetAllStorageAccounts()
        {

            return await _blobByStorageAcService.GetAllStorageAccounts();

        }


        [HttpGet]
        [Route("GetStorageContainer")]
        public async Task<IActionResult> GetStorageContainer(string storageAccountName)
        {

            return await _blobByStorageAcService.GetAllAccountContainer(storageAccountName);

        }

        [HttpGet]
        [Route("GetContainerBlob")]
        public async Task<IActionResult> GetContainerBlob(string storageAccountName, string containerName)
        {

            return await _blobByStorageAcService.GetAllContainerBlob(storageAccountName,containerName);

        }

        [HttpGet]
        [Route("GetBlobSasUrl")]
        public async Task<IActionResult> GetBlobSasUrl(string storageAccountName, string containerName, string blobName)
        {

            return await _blobByStorageAcService.GetBlobSasUrl(storageAccountName, containerName, blobName);

        }


        [HttpGet]
        [Route("downloadBlob")]
        public async Task<IActionResult> downloadBlob(string storageAccountName, string containerName, string blobName)
        {

            return await _blobByStorageAcService.downloadBlob(storageAccountName, containerName, blobName);

        }

        [HttpGet]
        [Route("GetSasUrl")]
        public async Task<IActionResult> GetSasUrl(string storageAccountName, string containerName, string blobName)
        {

            return await _blobByStorageAcService.GetSASUrl(storageAccountName, containerName,blobName);

        }
    }
}
