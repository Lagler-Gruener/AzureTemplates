using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;

namespace DemoAPI.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(string upn)
        {
            using (var domainContext = new PrincipalContext(ContextType.Domain, "demo.at", "DC=demo,DC=at"))
            {
                using (var foundUser = UserPrincipal.FindByIdentity(domainContext, System.DirectoryServices.AccountManagement.IdentityType.SamAccountName, upn))
                {
                    if (foundUser != null)
                    {
                        try
                        {
                            DirectoryEntry directoryEntry = foundUser.GetUnderlyingObject() as DirectoryEntry;
                            string strdepartment = directoryEntry.Properties["department"].Value.ToString();
                            string strcompany = directoryEntry.Properties["company"].Value.ToString();
                            //many details
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine(ex.Message);
                        }
                    }
                }
            }

            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
