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
using System.Collections.Generic;
using System.Text;

namespace DemoFunction
{
    public class Item
    {
        public string sAMAccountName { get; set; }
        public string givenName { get; set; }
        public string sn { get; set; }
        public string distinguishedName { get; set; }
        public string userPrincipalName { get; set; }
        public string Error { get; set; }
    }

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
            string name = data?.name;

            log.LogInformation("Create body message:");

            List<Item> bodymsg = new List<Item>();
            bodymsg.Add(new Item
            {
                distinguishedName = "test12",
                givenName = "test",
                sAMAccountName = "test12",
                sn = "test",
                userPrincipalName = "test12@demo.at"
            });

            var mycontent = JsonConvert.SerializeObject(bodymsg);

            log.LogInformation("Create new User:");

            var content = new StringContent(mycontent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await HttpClient.PostAsync("http://demodc01/API/Values/CreateUser", content);
            string body = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(body);
        }
    }
}
