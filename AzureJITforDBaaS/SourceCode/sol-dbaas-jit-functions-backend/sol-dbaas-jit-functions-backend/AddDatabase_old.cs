using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using sol_dbaas_jit_functions_backend.Class;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;

namespace sol_dbaas_jit_functions_backend
{
    public static class AddDatabase_old
    {
        [FunctionName("AddDatabase")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Execute Function AddDatabase");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string[] targetsarr = data?.data.essentials.alertTargetIDs.ToObject<string[]>();
            string[] targetsplit = targetsarr[0].ToString().Split(new string("/"), StringSplitOptions.RemoveEmptyEntries);

            string resourcegroup = targetsplit[3];
            string sqlsrv = targetsplit[7];
            string database = targetsplit[(targetsplit.Length - 1)];

            log.LogInformation("ResourceGroup:" + resourcegroup);
            log.LogInformation("SQLServer:" + sqlsrv);
            log.LogInformation("Database:" + database);


            /*
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation("FullBody:" + requestBody);

            JToken dbresresult = await getazressources();

            foreach (var entry in dbresresult["DBSrv"])
            {
                AzSQL classsql = new AzSQL();
                bool addresult = await classsql.AddDatabase((string)entry["name"],
                                                            (string)entry["region"],
                                                            (string)entry["rgname"],
                                                            (string)entry["db"]);
            }            

            */
            string state = "executed";
            return (ActionResult)new OkObjectResult(state);
        }

        [HttpPost]
        private static async Task<JToken> getazressources()
        {
            //var azureBaseUrlsecred = Environment.GetEnvironmentVariable("GetAzureRessourceFunction", EnvironmentVariableTarget.Process);
            var azureBaseUrlsecred = "https://sol-dbaas-jit-function.azurewebsites.net/api/Get-Az-Ressources?code=nvesJZm7vWXmfBvNklsp0tRsPmFHzPYTfr70Fv2BjsHp/Q4N5cucrg==";

            string resp = "None";
            using (HttpClient client = new HttpClient())
            {
                String json = "{}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await client.PostAsync(azureBaseUrlsecred, content);
                resp = await res.Content.ReadAsStringAsync();
            }

            JToken result = JToken.Parse(resp);


            return result;
        }
    }
}
