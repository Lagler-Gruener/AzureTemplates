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

namespace DemoFunction
{
    public static class FunctionGetInfos
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        [FunctionName("FunctionDemoAPI-GetInformation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);


            string sAMAccountName = data?.sAMAccountName;

            if (sAMAccountName != "") 
            {
                log.LogInformation("Get details of user: " + sAMAccountName);
                HttpResponseMessage response = await HttpClient.GetAsync("http://demodc01:80/api/values/GETUser?sAMAccountName="+ sAMAccountName);
                string body = await response.Content.ReadAsStringAsync();
                return new OkObjectResult(body);
            }
            else
            {
                log.LogInformation("");
                HttpResponseMessage response = await HttpClient.GetAsync("http://demodc01:80/api/values/GETUsers");
                string body = await response.Content.ReadAsStringAsync();
                return new OkObjectResult(body);
            }          
        }
    }
}
