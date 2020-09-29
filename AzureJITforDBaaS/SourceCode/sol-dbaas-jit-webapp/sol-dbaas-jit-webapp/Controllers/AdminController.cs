using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using sol_dbaas_jit_webapp.Models;

namespace sol_dbaas_jit_webapp.Views.Admin
{
    public class AdminController : Controller
    {
        public async System.Threading.Tasks.Task<ActionResult> Admin()
        {
            JToken a = await getazressources();

            List<AdminModel> listressources = new List<AdminModel>();
            AdminModel azressource;
            
            foreach (var entry in a["DBSrv"])
            {
                azressource = new AdminModel();             
                azressource.Server = (string)entry["name"];
                azressource.Database = (string)entry["db"];
                listressources.Add(azressource);
            }
            
            return View(listressources);
        }

        [HttpPost]
        private async Task<JToken> getazressources()
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