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
	/// <summary>
	/// <para>This class maintains a dictionary of OleDbConnections backed by a file -
	/// it does not open the connections or maintain a pool of connections. That would be
	/// a bad idea since most oledb providers implement connection pooling themselves.</para>
	/// <para>It does validate connection strings as they are submitted - or rather it delegates
	/// this validation to the <code>OleDbConnectionStringBuilder</code>
	/// </para>
	/// </summary>
    public static class OleDbConnectionService 
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
		
        // a standard .net class representing a collection of connection settings
        private static ConnectionStringSettingsCollection cssc = null;
        
        // an internal cache of OleDbConnections that have so far been created.
        // Note that OleDbConnectionService does not know of care whether the connections are open or closed
        // it simply keeps a cache of the created objects indexed by the name of the ConnectionStringSettings.
        
        private static Dictionary<string, OleDbConnection> connections = 
			new Dictionary<string, OleDbConnection>();

        private static bool TryGet(string connectionName, out OleDbConnection conn)
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
        
        public static bool TryGetConnection(string connectionName, out OleDbConnection connection)
		{
			if (connections.TryGetValue(connectionName, out connection)) {
				return true;
			} else {
				if (TryGet(connectionName, out connection)) {
					connections.Add(connectionName, connection);
					return true;
				}else {
        			return false;
        		} 
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
        public static Dictionary<string, OleDbConnection> Connections
        {
            get
            {
                // iterate through all connection strings trying to create a connection
                // from each. If an exception is thrown by the OleDbFactory then do not return it
                ConnectionStringSettingsCollection settingsCollection = GetConnectionSettingsCollection();
                List<OleDbConnection> results = new List<OleDbConnection>();
                foreach (ConnectionStringSettings c in settingsCollection)
                {
                	if (connections.ContainsKey(c.Name)) {
                		continue;
                	}
                    try
                    {
                        OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder(c.ConnectionString); // ArgumentException thrown here
                        OleDbConnection conn = new OleDbConnection(builder.ConnectionString);
                        connections.Add(c.Name, conn);
                    }
                    catch (ArgumentException)
                    {
                        log.Info("an invalid connection string was found in the file " +
                    	         CONNECTION_STRINGS_FILE + ": " + c.ToString());
                    }
                }
                return connections;
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
                log.Info("bad data found trying to read in the ConnectionSettingsCollection from file - " 
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
