using CDF_Services.IServices.IBlobStorageService;
using CDF_Services.Services.BlobStorageService;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.IO;

namespace CDF_WebApi.Controllers.BolbStorage
{

    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class BlobStorageController : Controller
    {
        private readonly IBlobStorageService _BlobStorageService;      


        public BlobStorageController(IBlobStorageService BlobStorageService)
        {
            _BlobStorageService = BlobStorageService;          
        }

       
        [HttpPost]
        [Route("uploadBlob_Identity")]
        public async Task<IActionResult> uploadDynamicBlob_Identity(IFormFile file)
        {
            return await _BlobStorageService.uploadDynamicBlobTest(file);

        }

        [HttpPost]
        [Route("downloadBlob_Identity")]
        public async Task<IActionResult> downloadBlob_Identity(string prefix)
        {
            return await _BlobStorageService.downloadBlobTest(prefix);

        }

        [HttpGet]
        [Route("ListBlobs_Identity")]
        public async Task<IActionResult> FilterBlobs(int? pageSize = 10, int? pageNumber = 1, string? period = null, string? reportingUnit = null, string? filename = null, string? containerName = "")
        {
            return await _BlobStorageService.FilterBlobs((int)pageSize, (int)pageNumber, period, reportingUnit, filename, containerName);

        }


        [HttpGet]
        [Route("getBLobSAS_Identity")]
        public async Task<IActionResult> getBLobSASIdentity(string BlobName)
        {

            return await _BlobStorageService.getBLobSASIdentity(BlobName);

        }

        [HttpGet]
        [Route("getBLobSasUrl_Dynamic")]
        public async Task<IActionResult> getBLobSasUrl(string storageAccountName, string containerName, string blobName)
        {

            return await _BlobStorageService.getBlobSasUrl_Dynamic(storageAccountName, containerName, blobName);

        }

        [HttpGet]
        [Route("getJsonFromBlob")]
        public async Task<IActionResult> getJsonFromBlob(string BlobName)
        {
            return Ok(await _BlobStorageService.ConvertToJsonFromUrl(BlobName));

        }
        private string ProcessExcelStream(Stream stream)
        {
            // Register encoding provider
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            DataSet excelData;
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                // Convert to DataSet with headers from the first row
                var conf = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                };

                excelData = reader.AsDataSet(conf);
            }

            // Convert DataSet to JSON
            var jsonResult = JsonConvert.SerializeObject(excelData, Formatting.Indented);
            return jsonResult;
        }
       /* [HttpGet("Convert-From-Url")]
        public async Task<IActionResult> ConvertFromUrl()
        {

            try
            {
                var url = "https://blobpoc02.blob.core.windows.net/demo/book1.xlsx";

                // Download the Excel file from the given URL
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        return BadRequest("Failed to download the Excel file.");
                    }

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        return Ok(ProcessExcelStream(stream));
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                return BadRequest($"Error downloading file: {ex.Message}");
            }
        }*/

      /*  [HttpPost("convert-from-text")]
        public IActionResult ConvertFromText(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                var headers=string.Empty;
                using (var reader = new StreamReader(file.OpenReadStream()))
                {


                    // Read the first line for headers (CSV column names)
                    headers = reader.ReadToEnd();

                }
                // Convert list of dictionaries to JSON
                var jsonResult = JsonConvert.SerializeObject(headers, Formatting.Indented);
                return Ok(jsonResult);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }*/



        /* [HttpPost("upload")]
         public IActionResult Upload(IFormFile file)
         {
             if (file == null || file.Length == 0)
             {
                 return BadRequest("No file uploaded.");
             }

             System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance); // Required for ExcelDataReader to handle different encodings

             DataSet excelData;

             using (var stream = new MemoryStream())
             {
                 file.CopyTo(stream);
                 stream.Position = 0;

                 using (var reader = ExcelReaderFactory.CreateReader(stream))
                 {
                     // Convert to DataSet (with each Excel sheet as a table)
                     var conf = new ExcelDataSetConfiguration
                     {
                         ConfigureDataTable = _ => new ExcelDataTableConfiguration
                         {
                             UseHeaderRow = true // Assumes first row is the header
                         }
                     };

                     excelData = reader.AsDataSet(conf);
                 }
             }

             // Convert DataSet to JSON
             var jsonResult = JsonConvert.SerializeObject(excelData, Formatting.Indented);

             return Ok(jsonResult);
         }


         [HttpGet("Convert-From-Url")]
         public async Task<IActionResult> ConvertFromUrl(string fileName)
         {

             try
             {
                 var url = "https://blobpoc02.blob.core.windows.net/demo/1001C_ABC_AOCI.xlsx";

                 // Download the Excel file from the given URL
                 using (HttpClient client = new HttpClient())
                 {
                     var response = await client.GetAsync(url);
                     if (!response.IsSuccessStatusCode)
                     {
                         return BadRequest("Failed to download the Excel file.");
                     }

                     using (var stream = await response.Content.ReadAsStreamAsync())
                     {
                         return Ok(ProcessExcelStream(stream));
                     }
                 }
             }
             catch (HttpRequestException ex)
             {
                 return BadRequest($"Error downloading file: {ex.Message}");
             }
         }

         [HttpGet("convert-from-base64")]
         public IActionResult ConvertFromBase64()
         {


             try
             {
                 var ExcelBase64Request = "";
                 // Decode Base64 string into a byte array
                 byte[] excelBytes = Convert.FromBase64String(ExcelBase64Request);

                 // Create a MemoryStream from the byte array
                 using (var stream = new MemoryStream(excelBytes))
                 {
                     // Process the stream to extract Excel data and convert to JSON
                     string jsonResult = ProcessExcelStream(stream);
                     return Ok(jsonResult);
                 }
             }
             catch (FormatException)
             {
                 return BadRequest("Invalid Base64 string.");
             }
         }

         private string ProcessExcelStream(Stream stream)
         {
             // Register encoding provider
             System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

             DataSet excelData;
             using (var reader = ExcelReaderFactory.CreateReader(stream))
             {
                 // Convert to DataSet with headers from the first row
                 var conf = new ExcelDataSetConfiguration
                 {
                     ConfigureDataTable = _ => new ExcelDataTableConfiguration
                     {
                         UseHeaderRow = true
                     }
                 };

                 excelData = reader.AsDataSet(conf);
             }

             // Convert DataSet to JSON
             var jsonResult = JsonConvert.SerializeObject(excelData, Formatting.Indented);
             return jsonResult;
         }

         [HttpPost("convert-from-text")]
         public IActionResult ConvertFromText(IFormFile file)
         {
             if (file == null || file.Length == 0)
             {
                 return BadRequest("No file uploaded.");
             }

             try
             {
                 List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

                 using (var reader = new StreamReader(file.OpenReadStream()))
                 {
                     // Read the first line for headers (CSV column names)
                     var headers = reader.ReadLine()?.Split(',');

                     if (headers == null)
                     {
                         return BadRequest("Invalid file format.");
                     }

                     // Read each line and convert it into a dictionary (key: header, value: data)
                     while (!reader.EndOfStream)
                     {
                         var line = reader.ReadLine();
                         if (line != null)
                         {
                             var values = line.Split(',');

                             var row = new Dictionary<string, string>();
                             for (int i = 0; i < headers.Length; i++)
                             {
                                 row[headers[i]] = values.Length > i ? values[i] : string.Empty;
                             }

                             result.Add(row);
                         }
                     }
                 }

                 // Convert list of dictionaries to JSON
                 var jsonResult = JsonConvert.SerializeObject(result, Formatting.Indented);
                 return Ok(jsonResult);
             }
             catch
             {
                 return StatusCode(500, "Internal server error");
             }
         }*/


        /*  [HttpPost]
          [Route("uploadDynamicBlob_Identity")]
          public async Task<IActionResult> uploadDynamicBlob_Identity(IFormFile file)
          {
              return await _BlobStorageService.uploadDynamicBlobTest(file);

          }



          [HttpPost]
          [Route("webhook")]
          public void ReceiveWebhook([FromBody] dynamic payload)
          {
             _BlobStorageService.ReceiveWebhook(payload);

          }

          [HttpGet]
          [Route("DownloadFile")]
          public async Task<IActionResult> DownloadFile(string prefix)
          {
              return await _BlobStorageService.DownloadFile(prefix);                

          }



          [HttpGet]
          [Route("getBLobSAS")]
          public async Task<IActionResult> getBLobSAS(string BlobName)
          {

             return await _BlobStorageService.getBLobSAS(BlobName);

          }


          [HttpGet]
          [Route("ListBlobsv3")]
          public async Task<IActionResult> ListBlobsv3(int? pageSize = 10)
          {
              return await _BlobStorageService.ListBlobsAsyncREST();

          }

          [HttpPost]
          [Route("uploadBlob")]
          public async Task<IActionResult> uploadBlob(IFormFile file, string? containerName = "container-poc")
          {
              return await _BlobStorageService.uploadBlob(file, containerName);   
          }

          [HttpPost]
          [Route("uploadBlobMultiple")]
          public async Task<IActionResult> uploadBlobMultiple(IFormFile file, int numberOfUploads = 10000, string? containerName = "container-poc")
          {
              return    await _BlobStorageService.uploadBlobMultiple(file, numberOfUploads, containerName); 
          }         
    */

    }
}

    

