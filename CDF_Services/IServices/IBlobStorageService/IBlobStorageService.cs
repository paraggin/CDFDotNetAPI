﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CDF_Services.IServices.IBlobStorageService
{
    public interface IBlobStorageService
    {

        Task<IActionResult> getBLobSAS(string BlobName);

        Task<IActionResult> getBLobSASIdentity(string BlobName);

        Task<IActionResult> getBlobSasUrl_Dynamic(string accountName, string containerName, string blobName);

        Task<IActionResult> DeleteBlob(string fileName);

        Task<string> ConvertToJsonFromUrl(string fileName);

        Task<IActionResult> FilterBlobs(int pageSize, int pageNumber, string period, string reportingUnit, string filename, string containerName);
      
        Task<IActionResult> ListBlobs_Identity(int pageSize, int pageNumber, string period, string reportingUnit, string filename, string containerName);
        

        Task<IActionResult> FilterBlobsUsingRestAPI(int pageSize, int pageNumber, string period, string reportingUnit, string filename, string containerName);


        Task<IActionResult> uploadBlob(IFormFile file, string containerName);

        Task<IActionResult> uploadDynamicBlobTest(IFormFile file);

        void ReceiveWebhook(dynamic payload);

        Task<IActionResult> uploadBlobMultiple(IFormFile file, int numberOfUploads, string containerName);

        Task<IActionResult> DownloadFile(string prefix);

        Task<IActionResult> downloadBlobTest(string prefix);

    }
}
