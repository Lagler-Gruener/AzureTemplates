using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.Threading;

namespace DemoAPI.Controllers
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

    public class ValuesController : ApiController
    {
        // GET api/values
        public List<Item> GetUsers()
        {
            DirectoryEntry rootDSE = new DirectoryEntry("LDAP://RootDSE");
            var defaultNamingContext = rootDSE.Properties["defaultNamingContext"].Value;

            //--- Code to use the current address for the LDAP and query it for the user---                  
            DirectorySearcher dssearch = new DirectorySearcher("LDAP://" + defaultNamingContext);
            dssearch.Filter = "(&(&(objectClass=user)(objectClass=person)))";
            SearchResultCollection sresult = dssearch.FindAll();

            List<Item> objectToSerialize = new List<Item>();

            if (null != sresult)
            {                
                foreach (SearchResult item in sresult)
                {                    
                    DirectoryEntry dsresult = item.GetDirectoryEntry();
                    string sAMAccountName = "null";
                    string givenName = "null";
                    string sn = "null";
                    string distinguishedName = "null";
                    string userPrincipalName = "null";

                    if (null != dsresult.Properties["sAMAccountName"].Value)
                    {
                        sAMAccountName = dsresult.Properties["sAMAccountName"].Value.ToString();
                    }
                    if (null != dsresult.Properties["givenName"].Value)
                    {
                        givenName = dsresult.Properties["givenName"].Value.ToString();
                    }
                    if (null != dsresult.Properties["sn"].Value)
                    {
                        sn = dsresult.Properties["sn"].Value.ToString();
                    }
                    distinguishedName = dsresult.Properties["distinguishedName"].Value.ToString();

                    if (null != dsresult.Properties["userPrincipalName"].Value)
                    {
                        userPrincipalName = dsresult.Properties["userPrincipalName"].Value.ToString();
                    }

                    objectToSerialize.Add(new Item
                    {
                        sAMAccountName = sAMAccountName,
                        givenName = givenName,
                        sn = sn,
                        distinguishedName = distinguishedName,
                        userPrincipalName = userPrincipalName
                    });

                }

                return objectToSerialize;
            }
            else
            {
                objectToSerialize.Add(new Item
                {
                    sAMAccountName = "null",
                    givenName = "null",
                    sn = "null",
                    distinguishedName = "null",
                    userPrincipalName = "null"
                });

                return objectToSerialize;
            }
        }

        // GET api/values/5
        public List<Item> GetUser(string sAMAccountName)
        {
            List<Item> objectToSerialize = new List<Item>();

            try
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "demo.at", "adminuser", "H01g1280!!!!!!");

                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, sAMAccountName);  

                if (null != user)
                {
                    objectToSerialize.Add(new Item
                    {
                        sAMAccountName = user.SamAccountName,
                        givenName = user.GivenName,
                        sn = user.Surname,
                        distinguishedName = user.DistinguishedName,
                        userPrincipalName = user.UserPrincipalName
                    });

                    return objectToSerialize;

                }
                else
                {
                    objectToSerialize.Add(new Item
                    {
                        sAMAccountName = "null",
                        givenName = "null",
                        sn = "null",
                        distinguishedName = "null",
                        userPrincipalName = "null"
                    });

                    return objectToSerialize;
                }

            }
            catch (Exception ex)
            {
                objectToSerialize.Add(new Item
                {
                    sAMAccountName = "null",
                    givenName = "null",
                    sn = "null",
                    distinguishedName = "null",
                    userPrincipalName = "null",
                    Error = ex.Message
                });

                return objectToSerialize;
            }                       
        }

        // POST api/values
        public string CreateUser([FromBody]Item value)
        {
            try
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "demo.at", "adminuser", "H01g1280!!!!!!");

                UserPrincipal user = new UserPrincipal(ctx);

                user.UserPrincipalName = value.userPrincipalName;
                user.SamAccountName = value.sAMAccountName;
                user.GivenName = value.givenName;
                user.Name = value.sn;
                user.Enabled = true;
                user.ExpirePasswordNow();
                user.Save();

                return "success";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // DELETE api/values/5
        public string DeleteUser([FromBody] Item value)
        {
            try
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "demo.at", "adminuser", "H01g1280!!!!!!");

                // find the user you want to delete
                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, value.sAMAccountName);

                if (user != null)
                {
                    user.Delete();
                    return "success";
                }
                else
                {
                    return "not found";
                }                

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
