[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(DemoWebinar.MVCGridConfig), "RegisterGrids")]

namespace DemoWebinar
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Linq;
    using System.Collections.Generic;
    using MVCGrid.Models;
    using MVCGrid.Web;
    using DemoWebinar.Models;
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class MVCGridConfig 
    {
        public static void RegisterGrids()
        {
            MVCGridDefinitionTable.Add("GridAllUsers", new MVCGridBuilder<ADItem>()
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .AddColumns(cols =>
                {
                    cols.Add("sAMAccountName").WithHeaderText("sAMAccountName")
                        .WithFiltering(true)
                        .WithValueExpression(p => p.sAMAccountName);
                    cols.Add("givenName").WithHeaderText("givenName")
                        .WithValueExpression(p => p.givenName);
                    cols.Add("sn").WithHeaderText("sn")
                        .WithValueExpression(p => p.sn);
                    cols.Add("distinguishedName").WithHeaderText("distinguishedName")
                        .WithValueExpression(p => p.distinguishedName);
                    cols.Add("userPrincipalName").WithHeaderText("userPrincipalName")
                        .WithValueExpression(p => p.userPrincipalName);
                })
                .WithFiltering(true)
                .WithRetrieveDataMethod((context) =>
                {
                    HttpClient HttpClient = new HttpClient();

                    var option = context.QueryOptions;
                    var deffilter = option.GetFilterString("Permission");

                    var result = new QueryResult<ADItem>();

                    var task = Task.Run(async () => await DemoWebinar.GetAllUsers());
                    var x = task.Result;

                    result.Items = task.Result;

                    return result;
                })
            );
        }
    }
}