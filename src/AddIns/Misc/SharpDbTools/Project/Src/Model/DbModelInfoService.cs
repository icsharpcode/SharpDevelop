// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision$</version>
// </file>

/*
 * User: dickon
 * Date: 28/07/2006
 * Time: 21:55
 * 
 */

using System;
using SharpDbTools.Connection;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace SharpDbTools.Model
{
	/// <summary>
	/// Manages a collection of DbModelInfo:
	/// - retrieval from files
	/// - opening (essentially refreshing) from a database connection
	/// - adding for new connection data (name, invariant name, connection string)
	/// - saving to files
	/// </summary>
	public class DbModelInfoService
	{
		static DbModelInfoService instance = new DbModelInfoService();
		
		SortedList<string, DbModelInfo> cache;
		
		DbModelInfoService()
		{
			cache = new SortedList<string, DbModelInfo>();
		}
		
		public static DbModelInfoService GetInstance()
		{
			return instance;
		}
		
		public DbModelInfo Create(string name, string invariantName, string connectionString)
		{
			// TODO: add validation on name; invariant name
			// assume that connection string is valid - if it fails an exception will be thrown later
			// this allows partially defined connection strings at least to be saved and worked on later
			
			DbModelInfo dbModel = new DbModelInfo(name, invariantName, connectionString);
			
			// add to cache
			cache.Add(name, dbModel);
			
			return dbModel;
		}
		
		public DbModelInfo LoadFromConnection(string name)
		{
			// get the DbModelInfo
			
			DbModelInfo modelInfo = null;
			bool exists = cache.TryGetValue(name, out modelInfo);
			if (!exists)
			{
				// TODO: add details to exception
				
				throw new KeyNotFoundException();
			}
			
			// get the invariant name and connection string
			
			string invariantName = modelInfo.InvariantName;
			string connectionString = modelInfo.ConnectionString;				
			
			// get a connection - wait until a connection has been successfully made
			// until clearing the DbModelInfo
			
			DbProvidersService factoryService = DbProvidersService.GetDbProvidersService();
			DbProviderFactory factory = factoryService[invariantName];
			DbConnection connection = factory.CreateConnection();
			
			// TODO: clear the DbModelInfo prior to refreshing from the connection
			
			// reload the metadata from the connection
			// get the Schema table
			
			connection.ConnectionString = connectionString;
			DataTable schemaInfo = connection.GetSchema();
			
			// iterate through the rows in it - the first column of each is a
			// schema info collection name that can be retrieved as a DbTable
			// Add each one to the DbModel DataSet
			
			foreach (DataRow collectionRow in schemaInfo.Rows) {
				String collectionName = (string)collectionRow[0];
				DataTable nextMetaData = connection.GetSchema(collectionName);
				modelInfo.Merge(nextMetaData);
			}
			return modelInfo;
		}
		
		public void Save(string name)
		{
			// TODO: save the 	
		}
		
		public void LoadFromFiles()
		{
			// TODO: load DbModelInfo's from file system
		}
		
		
	}
}
