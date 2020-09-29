using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using sol_dbaas_jit_functions_backend.Class;
using sol_dbaas_jit_functions_backend.Models;

namespace sol_dbaas_jit_functions_backend
{
    public static class AddDB
    {
        [FunctionName("AddDB")]
        public static async void Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            log.LogInformation($"Execute function AddDB");

            List<DatabaseModel> entriesfromazure = new List<DatabaseModel>();            
            AzureClass azurec = new AzureClass();
            entriesfromazure = await azurec.getazressources();

            List<DatabaseModel> entriesfromdb = new List<DatabaseModel>();
            AzSQL sqlc = new AzSQL();
            entriesfromdb = await sqlc.GetDatabase();

            //Check database for old entries
            foreach (DatabaseModel item in entriesfromdb)
            {
                if (entriesfromazure.Contains(item))
                {

                }
                else
                {

                }
            }

        }

       
    }
}
