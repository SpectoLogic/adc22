using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace midemo
{
    public static class MITest
    {
        [FunctionName("CreateBlob")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Create a new blob with managed identity!");
            try
            {
                // var mi = new Azure.Identity.DefaultAzureCredential();
                Azure.Identity.ManagedIdentityCredential mi = new Azure.Identity.ManagedIdentityCredential();

                string container = "demo";
                Azure.Storage.Blobs.BlobContainerClient containerClient = new Azure.Storage.Blobs.BlobContainerClient(new Uri($"https://midemo2.blob.core.windows.net/{container}"), mi);
                await containerClient.CreateIfNotExistsAsync();
                using Stream streamData = ToStream("Hello ADC-22!");
                await containerClient.UploadBlobAsync("demo.txt", streamData);

            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.ToString());
            }
            return new OkResult();
        }

        private static Stream ToStream(string text)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(text);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
