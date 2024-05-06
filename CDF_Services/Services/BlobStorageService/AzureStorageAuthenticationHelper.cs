using System;
using System.Net.Http.Headers;
using System.Text;

namespace CDF_Services.Services.BlobStorageService
{
    public static class AzureStorageAuthenticationHelper
    {
        public static AuthenticationHeaderValue GetAuthorizationHeader(string storageAccountName, string storageAccountKey, DateTime now, HttpRequestMessage requestMessage)
        {
            // Your implementation here to generate Azure Storage authentication header
            // This is just a placeholder
            string authToken = $"{storageAccountName}:{storageAccountKey}";
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(authToken)));
        }
    }
}
