using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DemoWebinar.Models;

namespace DemoWebinar.Controllers
{
    public class HomeController : Controller
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        #region Functions for function section

        public ActionResult DemoFunction()
        {
            ViewBag.sAMAccountName = TempData["sAMAccountName"];
            ViewBag.givenName = TempData["givenName"];
            ViewBag.sn = TempData["sn"];
            ViewBag.distinguishedName = TempData["distinguishedName"];
            ViewBag.userPrincipalName = TempData["userPrincipalName"];
            ViewBag.StateAddUser = TempData["StateAddUser"];
            ViewBag.StateDeleteUser = TempData["StateDeleteUser"];
            return View();
        }        

        public async Task<ActionResult> GetUser(FormCollection Config)
        {
            string samaccountname = Config["TextBoxRGName"];
            if (samaccountname.Length > 0)
            {
                var my_jsondata = new
                {
                    sAMAccountName = samaccountname
                };

                var mycontent = JsonConvert.SerializeObject(my_jsondata);

                HttpResponseMessage response = await HttpClient.PostAsync("https://demofunctiona01.azurewebsites.net/api/FunctionDemoAPI-GetInformation?code=qKylXGiQrEc90ggkJ9IqD/Q34Cx5JawOwinNbGbq1f4Xn6n9ugmElg==", new StringContent(mycontent, Encoding.UTF8, "application/json"));

                List<ADItem> result = null;

                var body = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<List<ADItem>>(body);

                TempData["sAMAccountName"] = result[0].sAMAccountName;
                TempData["givenName"] = result[0].givenName;
                TempData["sn"] = result[0].sn;
                TempData["distinguishedName"] = result[0].distinguishedName;
                TempData["userPrincipalName"] = result[0].userPrincipalName;

                return RedirectToAction("DemoFunction");
            }
            else
            {
                ModelState.AddModelError("DemoFunction", "Please enter a sAMAccountName");
                return RedirectToAction("DemoFunction");
            }
        }

        public async Task<ActionResult> AddUser(FormCollection Config)
        {
            string TextBoxAddUserSamAccountName = Config["TextBoxAddUserSamAccountName"];
            string TextBoxAddUsergivenname = Config["TextBoxAddUsergivenname"];
            string TextBoxAddUsersn = Config["TextBoxAddUsersn"];
            string TextBoxAddUseruserPrincipalName = Config["TextBoxAddUseruserPrincipalName"];

            List<ADItem> newuser = new List<ADItem>();
            newuser.Add(new ADItem
            {
                sAMAccountName = TextBoxAddUserSamAccountName,
                givenName = TextBoxAddUsergivenname,
                sn = TextBoxAddUsersn,
                userPrincipalName = TextBoxAddUseruserPrincipalName
            });

            string result = await DemoWebinar.Models.DemoWebinar.AddUsers(newuser);

            TempData["StateAddUser"] = result;

            return View("DemoFunction");
        }

        public async Task<ActionResult> RemoveUser(FormCollection Config)
        {
            string TextBoxDeleteUserSamAccountName = Config["TextBoxDelUserSamAccountName"];

            List<ADItem> removeuser = new List<ADItem>();
            removeuser.Add(new ADItem
            {
                sAMAccountName = TextBoxDeleteUserSamAccountName,
                givenName = "",
                sn = "",
                userPrincipalName = ""
            });

            string result = await DemoWebinar.Models.DemoWebinar.DeleteUsers(removeuser);

            TempData["StateDeleteUser"] = result;

            return View("DemoFunction");
        }

        #endregion
    }
}