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
	public class DbModelInfo: DataSet
	{
		public const string METADATACOLLECTIONS = "MetaDataCollections";
		
		public DbModelInfo()
		{
		}
		
		public DataTable MetaDataCollections {
			get {
				return this.Tables[METADATACOLLECTIONS];
			}
		}
		
		// TODO: an indexed property of MetaDataCollection
		//public 
	}
}
