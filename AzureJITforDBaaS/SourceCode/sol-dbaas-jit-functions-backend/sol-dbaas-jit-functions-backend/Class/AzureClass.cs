using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using sol_dbaas_jit_functions_backend.Models;

namespace sol_dbaas_jit_functions_backend.Class
{
    class AzureClass
    {
        public async Task<List<DatabaseModel>> getazressources()
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

            List<DatabaseModel> azdblist = new List<DatabaseModel>();
            DatabaseModel dbentry;

            foreach (var entry in result["DBSrv"])
            {
                dbentry = new DatabaseModel();
                dbentry.ServerName = ((string)entry["name"]).Trim();
                dbentry.Location = ((string)entry["region"]).Trim();
                dbentry.ResourceGroup = ((string)entry["rgname"]).Trim();
                dbentry.DatabaseName = ((string)entry["db"]).Trim();

                azdblist.Add(dbentry);
            }

            return azdblist;
        }
    }
}
