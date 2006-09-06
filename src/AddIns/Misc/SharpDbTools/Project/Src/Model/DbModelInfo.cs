// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision$</version>
// </file>

/*
 * User: Dickon Field
 * Date: 10/07/2006
 * Time: 09:12
 * 
 */

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace SharpDbTools.Model
{
	/// <summary>
	/// DbModel is a DataSet containing the metadata tables returned from a DbConnection.
	/// It adds methods designed specifically to facilitate access to the data in the
	/// DataTables contained by the DbModel.
	/// 
	/// The DbModel class is intended to be usable in a fully disconnected mode - that is,
	/// it requires a DbConnection to populate it, but it may then be locally persisted and subsequently
	/// retrieved from its persisted data. This is intended to allow work to progress against the
	/// DbModel without requiring a connection to its RDB server.
	/// </summary>
	/// 
	public class DbModelInfo: DataSet
	{
		public const string METADATACOLLECTIONS = "MetaDataCollections";
		
		public DbModelInfo() : base()
		{
		}
		
		public DbModelInfo(string name) : base()
		{
			DataTable table = CreateConnectionInfoTable();
			// add the first and only column of this table;
			table.Rows.Add(new object[] {name, null, null});		
		}
		
		public DbModelInfo(string name, string invariantName, string connectionString): base()
		{
			DataTable table = CreateConnectionInfoTable();
			// add the first and only column of this table;
			table.Rows.Add(new object[] {name, invariantName, connectionString});
		}
		
		public string Name {
			get {
				DataTable table = this.Tables[TableNames.ConnectionInfo];
				string name = table.Rows[0][ColumnNames.Name] as string;
				return name;
			}
		}
		
		public string InvariantName {
			get {
				DataTable table = this.Tables[TableNames.ConnectionInfo];
				string invariantName = null;
				try {
					invariantName = table.Rows[0][ColumnNames.InvariantName] as string;	
				}
				catch(ArgumentException) {
					// see comment below - it is correct to bury this exception
					//LoggingService.Info("InvariantName property was accessed while undefined" + e);
				}
				return invariantName;
			}
			set {
				DataTable table = this.Tables[TableNames.ConnectionInfo];
				string invariantName = table.Rows[0][ColumnNames.InvariantName] as string;
				string name = this.Name;
				string connectionString = this.ConnectionString;
				
				if (invariantName == null) {
					table.Rows[0][ColumnNames.InvariantName] = value;	
				}
				
				// if invariant has changed must clear any existing metadata
				else if (!(invariantName.Equals(value))) {
					// clear tables
					this.Clear();
					DataTable newTable = CreateConnectionInfoTable();
					// add the first and only column of this table;
					newTable.Rows.Add(new object[] {name, invariantName, connectionString});
				}				
			}
		}
		
		public string ConnectionString {
			get {
				DataTable table = this.Tables[TableNames.ConnectionInfo];
				string connectionString = null;
				try {
					connectionString = table.Rows[0][ColumnNames.ConnectionString] as string;	
				}
				catch(ArgumentException) {
					// this simply indicates that this attribute was not defined when the
					// DbModelInfo was saved, returning null makes sense here - so it is
					// correct to bury this exception
					//LoggingService.Info("InvariantName property was accessed while undefined" + e);
				}
				return connectionString;
			}
			set {
				DataTable table = this.Tables[TableNames.ConnectionInfo];
				string connectionString = this.ConnectionString;
				if (connectionString == null) {
					table.Rows[0][ColumnNames.ConnectionString] = value;	
				}
				else if (!(connectionString.Equals(value))) {
					string invariantName = this.InvariantName;
					string name = this.Name;
					this.Clear();
					// add the first and only column of this table;
					table.Rows.Add(new object[] {name, invariantName, value});
				}
			}
		}
		
		public void SetConnectionInfo(string invariantName, string connectionString)
		{
			string name = this.Name;
			SetConnectionInfo(name, invariantName, connectionString);
		}
		
		public void SetConnectionInfo(string name, string invariantName, 
		                              string connectionString)
		{
			this.Clear();
			DataTable table = CreateConnectionInfoTable();
			// add the first and only column of this table;
			table.Rows.Add(new object[] {name, invariantName, connectionString});			

		}
		
		private DataTable CreateConnectionInfoTable()
		{
			// create a table in the DbModelInfo to hold this initial info.
			// this creates a consistent representation of the data and makes
			// it easier to serialise it
			DataTable table = this.Tables.Add(TableNames.ConnectionInfo);
			table.Columns.Add(ColumnNames.Name, typeof(string));
			table.Columns.Add(ColumnNames.InvariantName, typeof(string));
			table.Columns.Add(ColumnNames.ConnectionString, typeof(string));
			return table;
		}
		
		public void ClearMetaData()
		{
			DataTable metadataCollectionsTable = this.MetaDataCollections;
			if (metadataCollectionsTable != null) {
				foreach (DataRow collectionRow in metadataCollectionsTable.Rows) {
					String collectionName = (string)collectionRow[0];
					this.Tables.Remove(collectionName);
				}
			}
		}
		
		public DataTable MetaDataCollections {
			get {
				return this.Tables[METADATACOLLECTIONS];
			}
		}
		
		public DataTable this[string collectionName] {
			get {
				return this.Tables[collectionName];
			}
		}
	}
}
