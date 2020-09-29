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

namespace sol_dbaas_jit_functions_backend
{
    public static class DeleteDatabase
    {
        [FunctionName("DeleteDatabase")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string[] targetsarr = data?.data.essentials.alertTargetIDs.ToObject<string[]>();
            string[] targetsplit = targetsarr[0].ToString().Split(new string("/"), StringSplitOptions.RemoveEmptyEntries);
            
            string resourcegroup = targetsplit[3];
            string sqlsrv = targetsplit[7];
            string database = targetsplit[(targetsplit.Length - 1)];

            log.LogInformation("Delete Database:");
            log.LogInformation("ResourceGroup:" + resourcegroup);
            log.LogInformation("SQLServer:" + sqlsrv);
            log.LogInformation("Database:" + database);

            AzSQL sqlclass = new AzSQL();
            bool deleteresult = await sqlclass.DeleteDatabase(sqlsrv, database);
            log.LogInformation("State:" + deleteresult);

            string state = "executed";
            return (ActionResult)new OkObjectResult(state);
        }
    }
}
