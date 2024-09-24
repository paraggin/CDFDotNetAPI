/*using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CDF_Services.IServices.IBlobStorageService;
using CDF_Services.Services.BlobStorageService;

namespace CDF_WebApi.Controllers.BolbStorage
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class BlobKeyVaultController : ControllerBase
    {
        private readonly IBlobKeyVaultService _blobKeyVaultService;
        private readonly IBlobStorageService _blobStorageService;

        public BlobKeyVaultController(IBlobKeyVaultService blobKeyVaultService, IBlobStorageService blobStorageService)
        {
            _blobKeyVaultService = blobKeyVaultService;
            _blobStorageService = blobStorageService;
        }


        [HttpGet]
        [Route("getBLobSasUrl_KeyVault")]
        public async Task<IActionResult> getBLobSasUrl(string storageAccountName, string containerName, string blobName)
        {

            return await _blobKeyVaultService.GenerateBlobSASUrl( storageAccountName,  containerName,  blobName);

        }

        [HttpGet]
        [Route("getBLobSasUrl")]
        public async Task<IActionResult> getBLobSasUrlDynamic(string storageAccountName, string containerName, string blobName)
        {

            return await _blobStorageService.getBlobSasUrl_Dynamic(storageAccountName, containerName, blobName);

        }

    }
}
*/