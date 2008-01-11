using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data.Common;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Xml.Serialization;
using ICSharpCode.Core;
using log = ICSharpCode.Core.LoggingService;



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
        /// 
        public const string CONNECTION_STRINGS_FILE = "Connections.conf";

        private static ConnectionStringSettingsCollection cssc;

        public static bool TryGet(string connectionName, out OleDbConnection conn)
        {
            try
            {
                ConnectionStringSettingsCollection settingsCollection = GetConnectionSettingsCollection();
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
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder(connectionString);
            ConnectionStringSettings st = new ConnectionStringSettings(name, connectionString);
            cssc.Add(st);
        }

        /// <summary>
        /// Returns all Connections unopened
        /// </summary>
        public static List<OleDbConnection> Connections
        {
            get
            {
                // iterate through all connection strings trying to create a connection
                // from each. If an exception is thrown by the OleDbFactory then do not return it
                ConnectionStringSettingsCollection settingsCollection = GetConnectionSettingsCollection();
                List<OleDbConnection> results = new List<OleDbConnection>();
                foreach (ConnectionStringSettings c in settingsCollection)
                {
                    try
                    {
                        OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder(c.ConnectionString); // ArgumentException thrown here
                        OleDbConnection conn = new OleDbConnection(builder.ConnectionString);
                        results.Add(conn);
                    }
                    catch (ArgumentException)
                    {
                        // do nothing, this is acceptable
                    }
                }
                return results;
            }
        }

        public static ConnectionStringSettingsCollection GetConnectionSettingsCollection()
        {
            if (cssc != null)
            {
                return cssc;
            }

            cssc = new ConnectionStringSettingsCollection();

            // Does the connections file exist - if not return an emptry collection
            string configPath = PropertyService.ConfigDirectory;
            string filePath = Path.Combine(configPath, CONNECTION_STRINGS_FILE);

            if (!File.Exists(filePath))
            {
                return cssc;
            }
            try
            {
                using (Stream sr = new FileStream(filePath, FileMode.Open))
                {
                    while (sr.Length > 0)
                    {
                        XmlSerializer x = new XmlSerializer(typeof(ConnectionStringSettings));
                        ConnectionStringSettings cs = (ConnectionStringSettings)x.Deserialize(sr);
                        cssc.Add(cs);
                    }
                }
            }
            catch (Exception)
            {
                // again, if the file contains some extraneous data it must have been corrupted or
                // manually edited - in this case ignore it
                log.Debug("bad data found trying to read in the ConnectionSettingsCollection from file - " 
                    + "please check the content of the file at: " + filePath);
            }
            return cssc;
        }
        

        /// <summary>
        /// Save whatever is in the cache to disc - overwriting whatever is there
        /// </summary>
        public static void Save()
        {
            string configPath = PropertyService.ConfigDirectory;
            string filePath = Path.Combine(configPath, CONNECTION_STRINGS_FILE);
            try
            {
                File.Delete(filePath);
            }
            catch (Exception)
            {
                // this should just indicate that the file does not exist
                log.Info("no connection settings file found while saving - will create a new one");
            }

            using (Stream sw = new FileStream(filePath, FileMode.CreateNew))
            {
                XmlSerializer xs = new XmlSerializer(typeof(ConnectionStringSettings));
                foreach (ConnectionStringSettings cs in cssc)
                {
                    try
                    {
                        xs.Serialize(sw, cs);
                    }
                    catch
                    {
                        log.Debug("failed to write ConnectionStringSettings: " + cs.ToString());
                    }
                }
            }
        }
    }
}
