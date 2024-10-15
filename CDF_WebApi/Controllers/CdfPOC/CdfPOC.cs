using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Text.Json;
using System.Net;
namespace CDF_WebApi.Controllers.CdfPOC
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class CdfPOC : ControllerBase
    {
     

        [HttpGet]
        [Route("ldagApi")]
        public async Task<IActionResult> ldagApi(string text)
        {

            try
            {
                // Create an HttpClientHandler that ignores SSL certificate validation
                var httpClientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using (HttpClient client = new HttpClient(httpClientHandler))
                {
                    string url = "https://ldag-stg.lazard.com/documents_plugin/laz_one_retrieval?requirement=" + text;

                    var postData = new { };

                    var json = JsonSerializer.Serialize(postData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string responseData = await response.Content.ReadAsStringAsync();

                    Console.WriteLine("POST Response Data:");
                    Console.WriteLine(responseData);
                    return new JsonResult(new { statusCode = 200, data = responseData });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling POST API: {ex.Message}");
                return new JsonResult(new { statusCode = 400, message = ex.Message });
            }
            /*
                        try
                        {

                            HttpClient client = new HttpClient();


                            string url = "https://ldag-stg.lazard.com/documents_plugin/laz_one_retrieval?requirement="+text; 

                            var postData = new{};

                            var json = JsonSerializer.Serialize(postData);

                            var content = new StringContent(json, Encoding.UTF8, "application/json");

                            HttpResponseMessage response = await client.PostAsync(url, content);

                            string responseData = await response.Content.ReadAsStringAsync();
                            Console.WriteLine("POST Response Data:");
                            Console.WriteLine(responseData);
                            return new JsonResult(new { statusCode = 200, data = responseData });


                        *//*    if (response.IsSuccessStatusCode)
                            {
                                string responseData = await response.Content.ReadAsStringAsync();
                                Console.WriteLine("POST Response Data:");
                                Console.WriteLine(responseData);
                                return new JsonResult(new { statusCode=200,data = responseData });
                            }
                            else
                            {

                                Console.WriteLine($"POST request failed with status code: {response.StatusCode}");
                                return new JsonResult(new {statusCode = response.StatusCode });

                            }*//*
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error calling POST API: {ex.Message}");
                            return new JsonResult(new { statusCode = 400, messsage = ex.Message });

                        }*/

        }
    }
}
