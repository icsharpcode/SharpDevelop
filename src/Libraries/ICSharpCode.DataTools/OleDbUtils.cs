using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data.Common;
using System.Data;
using System.Data.OleDb;


namespace ICSharpCode.DataTools
{
    public static class OleDbConnectionUtil
    {   
        /// <summary>
        /// Returns a single connection, unopened
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns>An OleDbConnection object if the connectioName refers to an oledb connection, otherwise null</returns>
        /// <exception cref="ArgumentException">thrown if the name of the connection is found, 
        /// but is not a valid oledb connection string</exception>
        public static bool TryGet(string connectionName, out OleDbConnection conn)
        {
            try
            {
                ConnectionStringSettingsCollection settingsCollection = getConnectionSettingsCollection();
                ConnectionStringSettings settings = settingsCollection[connectionName];
                string connString = settings.ConnectionString;
                OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder(connString); // ArgumentException thrown here
                conn = new OleDbConnection(builder.ConnectionString);
                return true;
            }
            catch (ArgumentException)
            {
                conn = null;
                return false;
            }
        }

        public static void Put(string name, string connectionString)
        {

            // check that it is an oledb connection string
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder(name);
            Configuration config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            ConnectionStringsSection s = config.ConnectionStrings;
            ConnectionStringSettingsCollection c = s.ConnectionStrings;
            ConnectionStringSettings st = new ConnectionStringSettings(name, connectionString);
            c.Add(st);
        }


        // TODO: test what happens when you set an OleDbConnection's connection string to a wrong value - probably
        // nothing until you try and open it? Could also try constructing and OleDbConnectionStringBuilder with the
        // string and see if it parses ok.

        /// <summary>
        /// Returns all Connections unopened
        /// </summary>
        public static List<OleDbConnection> Connections
        {
            get
            {
                // iterate through all connection strings trying to create a connection
                // from each. If an exception is thrown by the OleDbFactory then do not return it
                ConnectionStringSettingsCollection settingsCollection = getConnectionSettingsCollection();
                List<OleDbConnection> results = new List<OleDbConnection>();
                foreach (ConnectionStringSettings c in settingsCollection)
                {
                    try
                    {
                        OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder(c.ConnectionString); // ArgumentException thrown here
                        OleDbConnection conn = new OleDbConnection(builder.ConnectionString);
                        results.Add(conn);
                    }
                    catch(ArgumentException)
                    {
                        // do nothing, this is acceptable
                    }
                }
                return results;
            }
        }

        private static ConnectionStringSettingsCollection getConnectionSettingsCollection()
        {
            Configuration config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            ConnectionStringsSection connSection = config.ConnectionStrings;
            ConnectionStringSettingsCollection settingsCollection = connSection.ConnectionStrings;
            return settingsCollection;
        }
    }
}
