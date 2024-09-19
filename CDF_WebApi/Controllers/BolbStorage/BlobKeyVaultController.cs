using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CDF_Services.IServices.IBlobStorageService;

namespace CDF_WebApi.Controllers.BolbStorage
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class BlobKeyVaultController : ControllerBase
    {
        private readonly IBlobKeyVaultService _blobKeyVaultService;

        public BlobKeyVaultController(IBlobKeyVaultService blobKeyVaultService) {
            _blobKeyVaultService = blobKeyVaultService;
        }


        [HttpGet]
        [Route("getBLobSasUrl")]
        public async Task<IActionResult> getBLobSasUrl(string storageAccountName, string containerName, string blobName)
        {

            return await _blobKeyVaultService.GenerateBlobSASUrl( storageAccountName,  containerName,  blobName);

        }

    }
}
