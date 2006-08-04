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
		
		public string Name {
			get {
				DataTable table = this.Tables[TableNames.ConnectionInfo];
				string name = (string)table.Rows[0][ColumnNames.Name];
				return name;
			}
		}
		
		public string InvariantName {
			get {
				DataTable table = this.Tables[TableNames.ConnectionInfo];
				string invariantName = (string)table.Rows[0][ColumnNames.InvariantName];
				return invariantName;
			}
		}
		
		public string ConnectionString {
			get {
				DataTable table = this.Tables[TableNames.ConnectionInfo];
				string connectionString = (string)table.Rows[0][ColumnNames.InvariantName];
				return connectionString;
			}
		}
		
		public DbModelInfo() : base()
		{
		}
		
		public DbModelInfo(string name) : base()
		{
			DataTable table = CreateConnectionTable();
			// add the first and only column of this table;
			table.Rows.Add(new object[] {name, null, null});		
		}
		
		public DbModelInfo(string name, string invariantName, string connectionString): base()
		{
			DataTable table = CreateConnectionTable();
			// add the first and only column of this table;
			table.Rows.Add(new object[] {name, invariantName, connectionString});
		}
		
		private DataTable CreateConnectionTable()
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
