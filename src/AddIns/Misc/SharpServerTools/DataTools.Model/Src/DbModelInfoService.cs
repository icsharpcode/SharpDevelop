// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision: 1784 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;

using log4net;

namespace SharpDbTools.Data
{
	/// <summary>
	/// Manages a collection of DbModelInfo:
	/// - retrieval from files
	/// - opening (essentially refreshing) from a database connection
	/// - adding for new connection data (name, invariant name, connection string)
	/// - saving to files
	/// Note: it is not threadsafe
	/// </summary>
	public static class DbModelInfoService
	{
		const string dbFilesDir = "DbTools";	
		static SortedList<string, DbModelInfo> cache = null;
		static ILog log = LogManager.GetLogger(typeof(DbModelInfoService));
		static string savePath;
		
		
		public static string SavePath {
			set {
				savePath = value;
			}
			get {
				return savePath;
			}
		}
		public static IList<string> Names {
			get {
				if (cache == null) {
					cache = new SortedList<string, DbModelInfo>();
					LoadNamesFromFiles();
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
			log.Debug("-->GetTableInfo");
			DbModelInfo modelInfo = GetDbModelInfo(modelName);
			DataTable columnTable = modelInfo.Tables[MetadataNames.Columns];
			DataRow[] columnsMetadata = columnTable.Select(ColumnNames.TableName + "='" + tableName + "'");
			log.Debug("found: " + columnsMetadata.Length + " columns belonging to table: " + tableName);
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
				if (schemaInfo != null) {
					log.Debug("retrieved schema info with " + schemaInfo.Rows.Count + " rows");
				}
				
				// clear the DbModelInfo prior to refreshing from the connection
				modelInfo.ClearMetaData();
	
				// iterate through the rows in it - the first column of each is a
				// schema info collection name that can be retrieved as a DbTable
				// Add each one to the DbModel DataSet
				
				foreach (DataRow collectionRow in schemaInfo.Rows) {
					String collectionName = (string)collectionRow[0];
					log.Debug("loading metadata for collection: " + collectionName);
					DataTable nextMetaData = connection.GetSchema(collectionName);
					modelInfo.Merge(nextMetaData);
				}
				log.Debug("completed load of metadata, committing changes");
				modelInfo.AcceptChanges();
				return modelInfo;
			}
			catch(Exception e) {
				log.Fatal("Exception caught while trying to retrieve database metadata: " + e);
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
				log.Debug("writing metadata to: " + filePath);
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
			log.Debug("looking for metadata files at: " + saveLocation);
			string[] files = Directory.GetFileSystemEntries(saveLocation);

			cache.Clear();
			for (int i = 0; i < files.Length; i++) {
				log.Debug("found to load metadata from: " + files[i]);
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
			log.Debug("loading DbModelInfo from filePath: " + filePath);
			DbModelInfo nextModel = new DbModelInfo();
			nextModel.ReadXml(filePath);
			return nextModel;
		}
		
		public static void LoadFromFile(string logicalConnectionName)
		{
			log.Debug("loading DbModelInfo for name: " + logicalConnectionName);
			string saveLocation = GetSaveLocation();
			string path = saveLocation + "\\" + logicalConnectionName + ".metadata";
			DbModelInfo info = LoadFromFileAtPath(path);
			cache.Remove(logicalConnectionName);
			cache.Add(logicalConnectionName, info);
		}
		
		private static string GetSaveLocation()
		{
			// append the path of the directory for saving Db files
			
			if (SavePath == null) {
				string configDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				SavePath = configDir + @"\" + dbFilesDir;
				SavePath = SavePath.Replace("/", @"\");	
			}
			if (!Directory.Exists(SavePath)) {
				Directory.CreateDirectory(@SavePath);
			}
			return SavePath;			
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
