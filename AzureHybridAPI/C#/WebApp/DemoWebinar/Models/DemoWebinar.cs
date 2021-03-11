using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DemoWebinar.Models
{
    public class ADItem
    {
        public string sAMAccountName { get; set; }
        public string givenName { get; set; }
        public string sn { get; set; }
        public string distinguishedName { get; set; }
        public string userPrincipalName { get; set; }
        public string Error { get; set; }
    }

    public class DemoWebinar
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public static async Task<List<ADItem>> GetAllUsers()
        {
            var my_jsondata = new
            {
                sAMAccountName = ""
            };

            var mycontent = JsonConvert.SerializeObject(my_jsondata);

            HttpResponseMessage response = await HttpClient.PostAsync("https://demofunctiona01.azurewebsites.net/api/FunctionDemoAPI-GetInformation?code=qKylXGiQrEc90ggkJ9IqD/Q34Cx5JawOwinNbGbq1f4Xn6n9ugmElg==", new StringContent(mycontent, Encoding.UTF8, "application/json"));

            List<ADItem> result = null;

            var body = await response.Content.ReadAsStringAsync();
            result = JsonConvert.DeserializeObject<List<ADItem>>(body);

            return result;
        }

        public static async Task<string> AddUsers(List<ADItem> newuser)
        {
            var my_jsondata = new
            {
                sAMAccountName = newuser[0].sAMAccountName,
                givenName = newuser[0].givenName,
                sn = newuser[0].sn,
                distinguishedName = "",
                userPrincipalName = newuser[0].userPrincipalName
            };

            var mycontent = JsonConvert.SerializeObject(my_jsondata);

            HttpResponseMessage response = await HttpClient.PostAsync("https://demofunctiona01.azurewebsites.net/api/FunctionDemoAPI-AddAction?code=qKylXGiQrEc90ggkJ9IqD/Q34Cx5JawOwinNbGbq1f4Xn6n9ugmElg==", new StringContent(mycontent, Encoding.UTF8, "application/json"));

            var body = await response.Content.ReadAsStringAsync();

            return body.ToString();
        }

        public static async Task<string> DeleteUsers(List<ADItem> newuser)
        {
            var my_jsondata = new
            {
                sAMAccountName = newuser[0].sAMAccountName,
            };

            var mycontent = JsonConvert.SerializeObject(my_jsondata);

            HttpResponseMessage response = await HttpClient.PostAsync("https://demofunctiona01.azurewebsites.net/api/FunctionDemoAPI-DeleteAction?code=qKylXGiQrEc90ggkJ9IqD/Q34Cx5JawOwinNbGbq1f4Xn6n9ugmElg==", new StringContent(mycontent, Encoding.UTF8, "application/json"));

            var body = await response.Content.ReadAsStringAsync();

            return body.ToString();
        }
    }
}