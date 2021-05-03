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
using System.Text;

namespace DemoFunction
{
    public static class FunctionAddAction
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        [FunctionName("FunctionDemoAPI-AddAction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string sAMAccountName = data?.sAMAccountName;
            string givenName = data?.givenName;
            string sn = data?.sn;
            string userPrincipalName = data?.userPrincipalName;

            log.LogInformation("Create body message:");

            var my_jsondata = new
            {
                distinguishedName = "",
                givenName = givenName,
                sAMAccountName = sAMAccountName,
                sn = sn,
                userPrincipalName = userPrincipalName
            };

            var mycontent = JsonConvert.SerializeObject(my_jsondata);

            log.LogInformation("Create new User:");

            HttpResponseMessage response  = await HttpClient.PostAsync("http://demodc01/API/Values/CreateUser", new StringContent(mycontent, Encoding.UTF8, "application/json"));

            string body = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(body);
        }
    }
}
