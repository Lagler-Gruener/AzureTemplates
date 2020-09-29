using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using sol_dbaas_jit_functions_backend.Models;

namespace sol_dbaas_jit_functions_backend.Class
{
    class AzSQL
    {
        public async Task<bool> AddDatabase(string sqlserver, string dbsrvlocation,
                                            string resourcegroup, string database)
        {
            try
            {
                AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

                var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                var secret = await keyVaultClient.GetSecretAsync("https://keyvaultjitdbaasaccess.vault.azure.net/secrets/JITDBaaSSQLConnectionString")
                                                 .ConfigureAwait(false);

                string connectionstring = secret.Value;

                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    try
                    {
                        connection.Open();

                        string cmdgetsql = "select count(*) from AzDBSrv where dbsrvname = '" + sqlserver + "'";
                        string cmdgetdb = "select count(*) from AzDB where dbsrvname = '" + sqlserver + "' and dbname = '" + database + "'";

                        using (SqlCommand command = new SqlCommand(cmdgetsql, connection))
                        {
                            var dbserverresult = command.ExecuteScalar();
                            if (dbserverresult.Equals(0))
                            {
                                string addsqlserver = "insert into AzDBSrv (dbsrvname,location, resgroup) " +
                                                      " VALUES ('" + sqlserver + "','" + dbsrvlocation + "','" + resourcegroup + "')";

                                using (SqlCommand commandadddbsrv = new SqlCommand(addsqlserver, connection))
                                {
                                    commandadddbsrv.ExecuteNonQuery();
                                }                                
                            }

                            using (SqlCommand commandgetdb = new SqlCommand(cmdgetdb, connection))
                            {
                                var dbresult = commandgetdb.ExecuteScalar();

                                if (dbresult.Equals(0))
                                {
                                    string addsqldb = "insert into AzDB (dbsrvname,dbname) " +
                                                      " VALUES ('" + sqlserver + "','" + database + "')";

                                    using (SqlCommand commandadddb = new SqlCommand(addsqldb, connection))
                                    {
                                        commandadddb.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        
                    }
                }
            }
            catch (Exception exp)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteDatabase(string sqlserver, string database)
        {
            try
            {
                AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

                var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                var secret = await keyVaultClient.GetSecretAsync("https://keyvaultjitdbaasaccess.vault.azure.net/secrets/JITDBaaSSQLConnectionString")
                                                 .ConfigureAwait(false);

                string connectionstring = secret.Value;

                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    try
                    {
                        connection.Open();

                        string cmddeletedb = "delte from AzDB " +
                                             "where dbsrvname = '" + sqlserver + "' and dbname = '" + database + "'";


                        using (SqlCommand command = new SqlCommand(cmddeletedb, connection))
                        {
                            var dbserverresult = command.ExecuteScalar();                            
                        }
                    }
                    catch (Exception exp)
                    {

                    }
                }
            }
            catch (Exception exp)
            {
                return false;
            }

            return true;
        }

        public async Task<List<DatabaseModel>> GetDatabase()
        {
            List<DatabaseModel> dblist = new List<DatabaseModel>();

            try
            {
                AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

                var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                var secret = await keyVaultClient.GetSecretAsync("https://keyvaultjitdbaasaccess.vault.azure.net/secrets/JITDBaaSSQLConnectionString")
                                                 .ConfigureAwait(false);

                string connectionstring = secret.Value;

                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    try
                    {
                        connection.Open();

                        string cmdgetdb = "Select [AzDBSrv].dbsrvname, [AzDBSrv].location, [AzDBSrv].resgroup, [AzDB].dbname " +
                                          "from[AzDBSrv] " +
                                          "LEFT JOIN[AzDB] ON[AzDBSrv].dbsrvname = [AzDB].dbsrvname " +
                                          "where [AzDB].markedasdelete = 0 and [AzDBSrv].markedasdelete = 0";


                        DatabaseModel dbentry;

                        using (SqlCommand command = new SqlCommand(cmdgetdb, connection))
                        {
                            var dbserverresultreader = command.ExecuteReader();

                            while (dbserverresultreader.Read())
                            {
                                dbentry = new DatabaseModel();
                                dbentry.ServerName = dbserverresultreader[0].ToString().Trim();
                                dbentry.Location = dbserverresultreader[1].ToString().Trim();
                                dbentry.ResourceGroup = dbserverresultreader[2].ToString().Trim();
                                dbentry.DatabaseName = dbserverresultreader[3].ToString().Trim();

                                dblist.Add(dbentry);
                            }

                        }
                    }
                    catch (Exception exp)
                    {

                    }
                }
            }
            catch (Exception exp)
            {
            }

            return dblist;
        }
    }
}
