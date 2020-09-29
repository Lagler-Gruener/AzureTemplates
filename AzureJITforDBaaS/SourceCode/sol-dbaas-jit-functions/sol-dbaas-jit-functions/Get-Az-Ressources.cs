using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.Fluent;
using System.Dynamic;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace sol_dbaas_jit_functions
{
    public static class GetAzRessources
    {
        [FunctionName("Get-Az-Ressources")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            Microsoft.Azure.Management.Fluent.IAzure azure;            
            var tenantid = Environment.GetEnvironmentVariable("AzureTenantID", EnvironmentVariableTarget.Process);
            var subscriptionid = Environment.GetEnvironmentVariable("AzureSubscriptionID", EnvironmentVariableTarget.Process);
            var clientid = Environment.GetEnvironmentVariable("AzureAutSPClientID", EnvironmentVariableTarget.Process);
            var clientsecred = Environment.GetEnvironmentVariable("AzureAuthSPSecred", EnvironmentVariableTarget.Process);

            //Debug Mode configuration
            /*
            var tenantid = "c4091905-68e5-4088-b43d-476ae7b152f8";
            var subscriptionid = "bb8e13db-cd67-4923-8aea-a4d66b65cf84";
            var clientid = "a1f0354a-07ea-4274-a553-fe07ba0e575f";
            var clientsecred = "2j/v6_]]srEpsXlhH11/kBQUhI4wwqU2";
            */

            //Debug configuration settings
            //log.LogInformation($"tenantid:{tenantid}");
            //log.LogInformation($"subscriptionid:{subscriptionid}");
            //log.LogInformation($"clientid:{clientid}");
            //log.LogInformation($"clientsecred:{clientsecred}");

            azure = await LogintoAzure(clientid, clientsecred, tenantid, subscriptionid);

            JObject a = GetAzureSQLRessources(azure);

            return (ActionResult)new OkObjectResult(a);
        }

        private static async Task<IAzure> LogintoAzure(string clientid, string clientsecred, string tenantid, string subscriptionid)
        {            
            var creds = SdkContext.AzureCredentialsFactory.FromServicePrincipal(clientid,
                                                                                clientsecred,
                                                                                tenantid,
                                                                                AzureEnvironment.AzureGlobalCloud);

            var azure = Microsoft.Azure.Management.Fluent.Azure
                .Configure()
                .Authenticate(creds)
                .WithSubscription(subscriptionid);

            return azure;
        }

        private static JObject GetAzureSQLRessources(IAzure azureconnection)
        {
            dynamic result = new ExpandoObject();

            IEnumerable<Microsoft.Azure.Management.Sql.Fluent.ISqlServer> sqlservers = azureconnection.SqlServers.List();

            var jsonObject = new JObject();
            dynamic azureressources = jsonObject;
            azureressources.Entered = DateTime.Now;
            azureressources.Type = "AzureSQL";

            foreach (var sqlserver in sqlservers)
            {
                IReadOnlyList<Microsoft.Azure.Management.Sql.Fluent.ISqlDatabase> dbs = sqlserver.Databases.List();

                azureressources.DBSrv = new JArray() as dynamic;
                dynamic database;

                foreach (var db in dbs)
                {
                    if (db.Name != "master")
                    {
                        database = new JObject();
                        database.name = sqlserver.Name;
                        database.db = db.Name;
                        database.region = sqlserver.RegionName;
                        database.connectionstring = "Server=tcp:" + sqlserver.FullyQualifiedDomainName + ",1433;" +
                                                    "Initial Catalog=" + db.Name + ";Persist Security Info=False;" +
                                                    "User ID=" + sqlserver.AdministratorLogin + ";Password={your_password};" +
                                                    "MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                        database.rgname = sqlserver.ResourceGroupName;

                        azureressources.DBSrv.Add(database);
                    }
                }
            }

            return jsonObject;
        }        
    }
}
