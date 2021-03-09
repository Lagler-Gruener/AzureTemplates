using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;

namespace HybridDemo
{    
    public static class HybridDemo
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            HttpResponseMessage response = await HttpClient.GetAsync("http://localapi:81/API/Values/Get");
            string body = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(body);
        }
    }
}
