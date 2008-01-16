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
        public const string CONNECTION_STRINGS_FILE = "DatabaseConnections.xml";

        private static ConnectionStringSettingsCollection cssc = null;

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
		
        /// <summary>
        /// Validates and adds an oledb connection string to the internal cache.
        /// Note that this does not persist it - call Save() immediately after this
        /// you want the connection string to be available between sessions.
        /// </summary>
        /// <param name="name">an arbitrary display for the connection which can be used to
        /// display the connection. There can therefore be multiple instances of the same
        /// connection in use.</param>
        /// <param name="connectionString"></param>
        public static void Put(string name, string connectionString)
        {
            // check that it is an oledb connection string
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder(connectionString);
            // if an exception wasn't thrown then its a valid connection string, so add it to
            // the cache
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

            // Does the connections file exist - if not return an empty collection
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
                        XmlSerializer x = new XmlSerializer(typeof(ConnectionStringSettingsXMLSerializerWrapper));
                        ConnectionStringSettingsXMLSerializerWrapper csw 
                        	= (ConnectionStringSettingsXMLSerializerWrapper)x.Deserialize(sr);
                        ConnectionStringSettings cs = csw.ConnectionStringSettings;
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
        	log.Debug("persisting oledb connection strings");
            string configPath = PropertyService.ConfigDirectory;
            string filePath = Path.Combine(configPath, CONNECTION_STRINGS_FILE);
            try
            {
            	log.Debug("existing file found, so deleting it to be replaced");
                File.Delete(filePath);
            }
            catch (Exception)
            {
                // this should just indicate that the file does not exist
                log.Debug("no connection settings file found while saving - will create a new one");
            }

            using (Stream sw = new FileStream(filePath, FileMode.CreateNew))
            {
                XmlSerializer xs = new XmlSerializer(typeof(ConnectionStringSettingsXMLSerializerWrapper));
                foreach (ConnectionStringSettings cs in cssc)
                {
                    try
                    {
                    	log.Debug("serialising as xml and storing: " + cs.ToString());
                    	ConnectionStringSettingsXMLSerializerWrapper csw 
                    		= new ConnectionStringSettingsXMLSerializerWrapper(cs);
                        xs.Serialize(sw, csw);
                    }
                    catch(Exception e)
                    {
                    	throw (e);
                    		// TODO: do something sensible with the exception if it occurs
                        //log.Debug("failed to write ConnectionStringSettings: " + cs.ToString());
                        //log.Debug(e.StackTrace);
                    }
                }
            }
        }
    }
    
    public class ConnectionStringSettingsXMLSerializerWrapper
    {
    	private ConnectionStringSettings settings;
    	
    	public ConnectionStringSettingsXMLSerializerWrapper()
    	{
    		
    	}
    	
    	public ConnectionStringSettingsXMLSerializerWrapper(ConnectionStringSettings settings)
    	{
    		this.settings = settings;
    	}
    	
    	public string Name {
    		get {
    			return settings.Name;
    		}
    		set {
				ConnectionStringSettings.Name = value;
    		}
    	}
    	
    	public string ConnectionString {
    		get {
    			return settings.ConnectionString;
    		}
    		set {
    			ConnectionStringSettings.ConnectionString = value;
    		}
    	}
    	
    	[XmlIgnore]
    	public ConnectionStringSettings ConnectionStringSettings {
    		get {
    			if (settings == null) {
    				settings = new ConnectionStringSettings();
    			}
    			return settings;
    		}
    	}
    }
}
