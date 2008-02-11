/*
 * Created by SharpDevelop.
 * User: dickon
 * Date: 15/01/2008
 * Time: 16:47
 * 
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Configuration;
using MyMeta;


using ICSharpCode.DataTools;
using log=ICSharpCode.Core.LoggingService;

namespace ICSharpCode.ServerTools
{
	/// <summary>
	/// Bridges between the GUI object world and the world of connection strings and db objects
	/// </summary>
	public class DbControlController
	{
		private DbConnectionsNode _dbConnectionsNode;
		
		public DbControlController(DbConnectionsNode dbConnectionsNode)
		{
			_dbConnectionsNode = dbConnectionsNode;	
		}
		/// <summary>
		/// Creates or refreshes the tree of database objects below the DbConnectionNode 
		/// </summary>
		/// <param name="dbNode"></param>
		public void UpdateDbConnectionNode(DbConnectionNode dbNode, ConnectionStringSettings s)
		{
			// update the TablesNode
			
			UpdateTablesNode(dbNode.TablesNode, s);
			
			// update the ViewsNode
					
		}
		
		public void UpdateTablesNode(TablesNode tablesNode, ConnectionStringSettings s)
		{
			// get a list of table names for the current connection to work with
			
			OleDbConnection connection = null;
			if (OleDbConnectionService.TryGetConnection(s.Name, out connection)) {
				// now use mymeta to retrieve table metadata
				dbRoot m = new dbRoot();
				bool connected = m.Connect(dbDriver.SQL, s.ConnectionString);
				if (!connected) {
					MessageBox.Show("Could not connect", "Connect failed");
					return;
				}
				IDatabases dbs = m.Databases;
				OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder(s.ConnectionString);
				String dbName = null;
				Object o = null;
				builder.TryGetValue("Initial Catalog", out o);
				dbName = (String)o;
				IDatabase db = dbs[dbName];
				ITables tables = db.Tables;
				foreach (ITable t in tables) {
					// yes, much TODO: this does not update, it just adds
					TableNode tableNode = new TableNode();
					tableNode.Header = t.Alias;
					tablesNode.Items.Add(tableNode);
				}			
			}		
		}
		
		public void UpdateTableNode(DbConnectionNode dbNode, string connectionName)
		{
			
		}
		
		/// <summary>
		/// Creates a DbConnectionNode below the 'Data Connections' node of the ServerControl.
		/// Simply iterate through the OleDbConnections available and:
		/// 	- if a DbConnectionNode does not exist for one then add it
		/// 	- if one does exist, then update its DbConnectionNodeState to correspond to the state of the
		/// 	OleDbConnection.
		/// </summary>
		/// <param name="dbConnectionsNode"></param>
		public void UpdateDbConnectionsNode()
		{
			log.Debug("updating connections node");
            ConnectionStringSettingsCollection c
            = OleDbConnectionService.GetConnectionSettingsCollection();
            
            // add a new DbConnectionNode for any connection that has been added
            List<string> headers = new List<string>();
            foreach (ConnectionStringSettings s in c)
            {
				string settingsName = s.Name;
				headers.Add(settingsName);
				DbConnectionNode dbNode = (DbConnectionNode)_dbConnectionsNode.GetItemWithHeader(settingsName);
				if (dbNode == null) {
					// initial state for a new connection is always closed I think
					dbNode = new DbConnectionNode(settingsName);
					_dbConnectionsNode.Items.Add(dbNode);
				} 
				UpdateDbConnectionNode(dbNode, s);
            }
            _dbConnectionsNode.RemoveItemsWithHeaderNotIn(headers);
		}
		
		        /// <summary>
        /// Uses the DataLink dialog to define a new Oledb connection string,
        /// test it, and adds it to config.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddConnectionButton_Clicked(object sender, RoutedEventArgs e)
        {
        	log.Debug("add connection button clicked handler");
            MSDASC.DataLinks mydlg = new MSDASC.DataLinks();
            OleDbConnection oleCon = new OleDbConnection();
            ADODB._Connection aDOcon;

            //Cast the generic object that PromptNew returns to an ADODB._Connection.
            aDOcon = (ADODB._Connection)mydlg.PromptNew();
            if (aDOcon == null)
            {
            	return;
            }

            oleCon.ConnectionString = aDOcon.ConnectionString;
            oleCon.Open();

            if (oleCon.State.ToString() == "Open")
            {
                // If we get to here, we have a valid oledb
                // connection string, at least on the basis of the current
                // state of the platform that it refers to.
                // Now construct a name for the connection string settings based on
                // the attributes of the connection string and save it.
                // VS08 assumes the following naming scheme:
                // connection name ::= <provider name>.<host name>\<server name>.<catalog name>
                
                string provider = oleCon.Provider;
                string source = oleCon.DataSource;
                string catalogue = oleCon.Database;
                string dbServerName = @provider + ":" + @source + "." + @catalogue;
                
                OleDbConnectionService.Put(dbServerName, oleCon.ConnectionString);
                OleDbConnectionService.Save();
                oleCon.Close();
            }
            else
            {
                MessageBox.Show("Connection Failed");
            }
            this.UpdateDbConnectionsNode();
        }
        
        public void RefreshButton_Clicked(object sender, EventArgs eventArgs)
        {
        	log.Debug("refresh button clicked handler");
        }
	}
}
