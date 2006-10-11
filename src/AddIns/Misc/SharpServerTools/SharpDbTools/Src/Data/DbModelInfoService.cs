// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision: 1784 $</version>
// </file>

/*
 * User: dickon
 * Date: 28/07/2006
 * Time: 21:55
 * 
 */

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;

namespace SharpDbTools.Data
{
	/// <summary>
	/// Manages a collection of DbModelInfo:
	/// - retrieval from files
	/// - opening (essentially refreshing) from a database connection
	/// - adding for new connection data (name, invariant name, connection string)
	/// - saving to files
	/// </summary>
	public static class DbModelInfoService
	{
		static string saveLocation = null;
		static object lockObject = new Object();
		const string dbFilesDir = "DbTools";	
		static SortedList<string, DbModelInfo> cache = null;
		
		public static IList<string> Names {
			get {
				lock(lockObject) {
					if (cache == null) {
						cache = new SortedList<string, DbModelInfo>();
						LoadNamesFromFiles();
					}
				}
				return cache.Keys;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">The user readable name of the provider</param>
		/// <param name="invariantName">the identifying name of the provider</param>
		/// <param name="connectionString">the connection string for this connection</param>
		/// <returns></returns>
		public static DbModelInfo Add(string name, string invariantName, string connectionString)
		{
			// TODO: add validation on name; invariant name
			// assume that connection string is valid - if it fails an exception will be thrown later
			// this allows partially defined connection strings at least to be saved and worked on later
			DbModelInfo dbModel = new DbModelInfo(name, invariantName, connectionString);
			
			// add to cache
			cache.Add(name, dbModel);
			return dbModel;
		}
		
		public static void Remove(string name)
		{
			cache.Remove(name);
		}
		
		public static DbModelInfo GetDbModelInfo(string name) {
				DbModelInfo modelInfo = null;
				bool exists = cache.TryGetValue(name, out modelInfo);
				return modelInfo;
		}
		
		public static DataTable GetTableInfo(string modelName, string tableName)
		{
			LoggingService.Debug("-->GetTableInfo");
			DbModelInfo modelInfo = GetDbModelInfo(modelName);
			DataTable columnTable = modelInfo.Tables[TableNames.Columns];
			DataRow[] columnsMetadata = columnTable.Select(ColumnNames.TableName + "='" + tableName + "'");
			LoggingService.Debug("found: " + columnsMetadata.Length + " columns belonging to table: " + tableName);
			DataTable tableInfo = new DataTable();
			DataColumnCollection cols = columnTable.Columns;
			foreach (DataColumn c in cols) {
				DataColumn nc = new DataColumn(c.ColumnName, c.DataType);	
				tableInfo.Columns.Add(nc);
			}
			foreach (DataRow r in columnsMetadata) {
				DataRow newRow = tableInfo.NewRow();
				newRow.ItemArray = r.ItemArray;
				tableInfo.Rows.Add(newRow);
				tableInfo.AcceptChanges();
			}
			return tableInfo;

		}
				
		public static DbModelInfo LoadMetadataFromConnection(string name)
		{
			// get the DbModelInfo
			
			DbModelInfo modelInfo = null;
			bool exists = cache.TryGetValue(name, out modelInfo);
			if (!exists)
			{
				// TODO: more detail...
				throw new KeyNotFoundException();
			}
			
			// get the invariant name and connection string
			
			string invariantName = modelInfo.InvariantName;
			string connectionString = modelInfo.ConnectionString;				
			
			// get a connection - wait until a connection has been successfully made
			// before clearing the DbModelInfo
			
			DbProvidersService factoryService = DbProvidersService.GetDbProvidersService();
			DbConnection connection = null;
			try {
				
				DbProviderFactory factory = factoryService.GetFactoryByInvariantName(invariantName);
				connection = factory.CreateConnection();
				
				
				modelInfo.ClearMetaData();
				
				// reload the metadata from the connection
				// get the Schema table
				connection.ConnectionString = connectionString;
	
				connection.Open();
				DataTable schemaInfo = connection.GetSchema();
				
				// clear the DbModelInfo prior to refreshing from the connection
				modelInfo.ClearMetaData();
	
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
			catch(Exception e) {
				LoggingService.Fatal("Exception caught while trying to retrieve database metadata: " + e);
				throw e;
			}
			finally {
				connection.Close();
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">the logical name of the DbModelInfo to save to a file</param>
		/// <param name="overwriteExistingFile">if true, any existing file will be overwritten</param>
		/// <returns>true if the DbModelInfo was saved, false if not. It will not be saved if
		/// either overwriteExistingFile is set to true, and there is an existing file</returns>
		public static bool SaveToFile(string name, bool overwriteExistingFile)
		{
			string path = GetSaveLocation();
			DbModelInfo modelInfo = null;
			cache.TryGetValue(name, out modelInfo);
			if (modelInfo != null) {
				string modelName = modelInfo.Name;
				// write to a file in 'path' called <name>.metadata
				// TODO: may want to consider ways of making this more resilient
				
				string connectionProps = modelInfo.ConnectionString;
				string invariantName = modelInfo.InvariantName;
				
				
				
				string filePath = path + @"\" + name + ".metadata";
				LoggingService.Debug("writing metadata to: " + filePath);
				if (File.Exists(filePath)) {
				    	if (overwriteExistingFile) {
				    		File.Delete(filePath);	
				    	} else {
				    		return false;
				    	}
				}
				using (StreamWriter sw = File.CreateText(filePath)) {
					string xml = modelInfo.GetXml();
					sw.Write(xml);
					sw.Flush();
					sw.Close();
					return true;
				}
			} else {
				throw new DbModelInfoDoesNotExistException(name);
			}
		}
		
		public static void SaveAll()
		{
			foreach (string name in cache.Keys) {
				SaveToFile(name, true);
			}
		}
		
		public static void LoadNamesFromFiles()
		{
			// load DbModelInfo's from file system
			string saveLocation = GetSaveLocation();
			LoggingService.Debug("looking for metadata files at: " + saveLocation);
			string[] files = Directory.GetFileSystemEntries(saveLocation);

			cache.Clear();
			for (int i = 0; i < files.Length; i++) {
				LoggingService.Debug("found to load metadata from: " + files[i]);
				int start = files[i].LastIndexOf('\\');
				int end = files[i].LastIndexOf('.');
				start++;
				string name = files[i].Substring(start, end - start);
				DbModelInfo nextModel = new DbModelInfo(name);
				cache.Add(nextModel.Name, nextModel);
			}	
		}
		
		public static void LoadFromFiles()
		{
			// load DbModelInfo's from file system
			string saveLocation = GetSaveLocation();
			string[] files = Directory.GetFiles(saveLocation);
			cache.Clear();
			for (int i = 0; i < files.Length; i++) {
				DbModelInfo nextModel = LoadFromFileAtPath(@files[i]);
				cache.Add(nextModel.Name, nextModel);
			}
		}
		
		private static DbModelInfo LoadFromFileAtPath(string filePath)
		{
			LoggingService.Debug("loading DbModelInfo from filePath: " + filePath);
			DbModelInfo nextModel = new DbModelInfo();
			nextModel.ReadXml(filePath);
			return nextModel;
		}
		
		public static void LoadFromFile(string logicalConnectionName)
		{
			LoggingService.Debug("loading DbModelInfo for name: " + logicalConnectionName);
			string saveLocation = GetSaveLocation();
			string path = saveLocation + "\\" + logicalConnectionName + ".metadata";
			DbModelInfo info = LoadFromFileAtPath(path);
			cache.Remove(logicalConnectionName);
			cache.Add(logicalConnectionName, info);
		}
		
		private static string GetSaveLocation()
		{
			// append the path of the directory for saving Db files
			
			if (saveLocation == null) {
				lock(lockObject) {
					string configDir = PropertyService.ConfigDirectory;
					saveLocation = configDir + @"\" + dbFilesDir;
					saveLocation = saveLocation.Replace("/", @"\");
				}
			}
			if (!Directory.Exists(saveLocation)) {
				Directory.CreateDirectory(@saveLocation);
			}
			return saveLocation;			
		}
	}
	
	public class DbModelInfoDoesNotExistException: ApplicationException
	{
		string name;
		
		public DbModelInfoDoesNotExistException(string dbModelInfoName): base()
		{
			this.name = dbModelInfoName;	
		}
		
		public string Name {
			get {
				return name;
			}
		}
	}
}
