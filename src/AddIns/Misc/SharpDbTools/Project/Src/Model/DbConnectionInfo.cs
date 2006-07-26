/*
 * User: Dickon Field
 * Date: 05/07/2006
 * Time: 22:13
 * 
 */

using System;
using System.Data;
using System.Data.Common;
using ICSharpCode.Core;


namespace SharpDbTools.Model
{
	/// <summary>
	/// DbConnectionInfo object is in essence a client side cache of database
	/// connection information that can be used disconnected from the database
	/// for query editing and validation, database browsing etc.
	/// 
	/// It is persisted as an XML element into local storage, and can subsequently
	/// be either used independently or refreshed from the database server.
	/// 
	/// </summary>
	public class DbConnectionInfo: IConnectionInfo, IDisposable
	{
		DbConnection connection = null;
		string connectionString = null;
		string invariantName = null;
		string name = null;
		DbModelInfo dbModel = null;
		
		private DbConnectionInfo()
		{
			
		}
		
		public DbConnectionInfo(string name, string invariantName, string connectionString)
		{
			this.name = name;
			this.connectionString = connectionString;
			this.invariantName = invariantName;
		}
		
		public string Name {
			get {
				return name;
			}
		}
		
		public string InvariantName {
			get {
				return invariantName;
			}
		}
		
		public string ConnectionString {
			get {
				return connectionString;
			}
		}
		
		public bool HasConnection {
			get {
				return !(connection == null);
			}
		}
		
		public bool HasModel {
			get { 
				return !(dbModel == null);
			}
		}
		
		public DbConnection Connection {
			get {
				// return connection if defined else try and create it
				if (connection != null) {
					return connection;
				}
				else {
					DbProviderFactory factory = DbProviderFactories.GetFactory(invariantName);
					this.connection = factory.CreateConnection();
					this.connection.ConnectionString = this.connectionString;
					this.connection.Open();
					return this.connection;
				}
			}			
		}
		
		public DataSet DbModel {
			get
			{
				if (dbModel == null)
				{
					dbModel = new DbModelInfo();					
					DbConnection connection = this.Connection;
					
					// get the Schema table
					
					DataTable schemaInfo = connection.GetSchema();
					
					// iterate through the rows in it - the first column of each is a
					// schema info collection name that can be retrieved as a DbTable
					// Add each one to the DbModel DataSet
					
					foreach (DataRow collectionRow in schemaInfo.Rows) {
						String collectionName = (string)collectionRow[0];
						DataTable nextMetaData = connection.GetSchema(collectionName);
						dbModel.Merge(nextMetaData);
					}
						
				}
				return this.dbModel;
			}
		}
		
		public void Dispose()
		{
			try {
				this.connection.Close();
			}
			catch(Exception e) {
				LoggingService.Warn("unable to close connection: exception thrown", e);
			}
		}
		
		// TODO: serialise into a store
	}
}
