// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Luc Morin" email=""/>
//     <version>$Revision$</version>
// </file>

using System.Data;
using SharpQuery.Collections;
using SharpQuery.SchemaClass;

namespace SharpQuery.Connection
{
	///<summary>
	/// This interface is used by any connection wrapper class.
	///</summary>
	public interface IConnection
	{
		///<summary>
		/// return <c>true</c> if the connection string is invalid.
		///</summary>
		bool IsConnectionStringWrong
		{
			get;
		}
		///<summary>Return the connection string.</summary>
		string ConnectionString { get; set;}

		///<summary>Provider'sname.
		/// </summary>
		string Provider { get; }

		///<summary>
		/// get the <code>connection</code>'s <code>catalog</code> name.
		///</summary>
		string CatalogName { get; }

		///<summary>
		/// get the <code>connection</code>'s <code>schema</code> name.
		///</summary>
		string SchemaName { get; }

		///<summary>
		/// get the <code>entity</code>'s name
		///</summary>
		string Name { get; }

		///<summary>
		/// <code>entity</code>'s normalized name.
		/// <example>
		/// there are a database <code>test</code>. In this database there are a <code>schema</code>
		/// named <code>fool</code>. In this <code>schema</code> there are a <code>table</code> named <code>fooltable</code>
		/// and a in this <code>table</code> there are a <code>column</code> named <code>foolcolumn</code>.
		/// So, the normalized name of the <code>schema</code> is <code>test.[fool]</code>.
		/// The normalized name of the <code>table</code> is <code>test.[fool].fooltable</code>.
		/// The normalized name of the <code>column</code> is <code>test.[fool].fooltable.foolcolumn</code>.
		///</example>
		///</summary>
		string NormalizedName { get; }

		///<summary>
		/// <returns>
		///	<code>true</code> if the connection is opened.
		/// <code>false</code> if the connection is not opened.
		/// </returns>
		/// </summary>
		bool IsOpen { get; }

		///<summary>
		/// Those, are list of the childs schema.( columns, etc etc )
		/// i am using a dictionnary because is more simplest to write
		/// <code>Entities["PROCEDURES"]</code> than <code>Entities[0]</code>.
		///</summary>
		SharpQueryListDictionary Entities { get; }

		///<summary>
		/// <returns>
		///	an array of string with the string properties's provider
		/// </returns>
		/// <remarks> properties are indexed ans sorted with <see cref=" SharpQuery.SchemaClass.AbstractSharpQueryConnectionWrapper.SharpQueryPropertyEnum">SharpQueryPropertyEnum</see></remarks>
		/// </summary>
		object GetProperty(AbstractSharpQueryConnectionWrapper.SharpQueryPropertyEnum property);

		///<summary>
		/// Open a connection with the <seealso cref='.IConnection.ConnectionString'/>
		/// <returns>
		///	<code>true</code> if the connection is opened.
		/// <code>false</code> if the connection is not opened.
		/// </returns>
		/// </summary>
		bool Open();

		///<summary>
		/// Close the connection with the server.
		/// </summary>
		void Close();

		///<summary>
		/// Refresh child schemas from the database.
		///</summary>
		void Refresh();

		///<summary>
		/// Erase all child schemas
		///</summary>
		void Clear();

		///<summary>
		/// Execute a SQL command
		/// <param name="SQLText">
		/// SQL command to execute
		/// </param>
		/// <param name="rows">
		/// Maximum number of row to extract. If is "0" then all rows are extracted.
		/// </param>
		/// <returns> return a <see cref="System.Data.DataTable">DataTable</see>
		///or a <see cref="System.Data.DataSet">DataSet</see> object.
		/// </returns>
		/// </summary>
		object ExecuteSQL(string SQLText, int rows);

		///<summary>
		/// Execute a stocked procedure.
		/// <param name="schema">
		/// <see cref="SharpQuery.SchemaClass">SchemaClass</see> object.
		/// </param>
		/// <param name="rows">
		/// Maximum number of row to extract. If is "0" then all rows are extracted.
		/// </param>
		/// <returns> return a <see cref="System.Data.DataTable">DataTable</see>
		///or a <see cref="System.Data.DataSet">DataSet</see> object.
		/// </returns>
		/// </summary>
		object ExecuteProcedure(ISchemaClass schema, int rows, SharpQuerySchemaClassCollection parameters);

		///<summary>
		/// Extract Data from a Table or a View
		/// <param name="schema">
		/// <see cref="SharpQuery.SchemaClass">SchemaClass</see> object.
		/// </param>
		/// <param name="rows">
		/// Maximum number of row to extract. If is "0" then all rows are extracted.
		/// </param>
		/// <returns> return a <see cref="System.Data.DataTable">DataTable</see>
		///or a <see cref="System.Data.DataSet">DataSet</see> object.
		/// </returns>
		/// </summary>
		object ExtractData(ISchemaClass schema, int rows);

		///<summary>
		/// Update <see cref="System.Data.DataRow">row</see>'s fields into the current opened database.
		/// <param name="row">a <see cref="System.Data.DataRow">row</see> </param>
		/// <param name="schema"> a <see cref="SharpQuery.SchemaClass.ISchema">schema</see> </param>
		///</summary>
		void UpDateRow(ISchemaClass schema, DataRow row);

		///<summary>
		/// Delete <see cref="System.Data.DataRow">row</see> into the current opened database.
		/// <param name="row">a <see cref="System.Data.DataRow">row</see> </param>
		/// <param name="schema"> a <see cref="SharpQuery.SchemaClass.ISchema">schema</see> </param>
		///</summary>
		void DeleteRow(ISchemaClass schema, DataRow row);

		///<summary>
		/// Insert <see cref="System.Data.DataRow">row</see> into the current opened database.
		/// <param name="row">a <see cref="System.Data.DataRow">row</see> </param>
		/// <param name="schema"> a <see cref="SharpQuery.SchemaClass.ISchema">schema</see> </param>
		///</summary>
		void InsertRow(ISchemaClass schema, DataRow row);


		///<summary>
		/// Get <seealso cref='.ISchemaClass.Connection'/>'s catalogs.
		///</summary>
		SharpQuerySchemaClassCollection GetSchemaCatalogs(ISchemaClass schema);

		///<summary>
		/// Get <seealso cref='.ISchemaClass.Connection'/>'s Schemas.
		///</summary>
		SharpQuerySchemaClassCollection GetSchemaSchemas(ISchemaClass schema);

		///<summary>
		/// From a catalog object, get tables from all schemas.
		/// From a schema object get tables from all this schema.
		/// From other object, return an empty list.
		///</summary>
		SharpQuerySchemaClassCollection GetSchemaTables(ISchemaClass schema);

		///<summary>
		/// From a catalog object, get views from all schemas.
		/// From a schema object get views from all this schema.
		/// From other object, return an empty list.
		///</summary>
		SharpQuerySchemaClassCollection GetSchemaViews(ISchemaClass schema);

		///<summary>
		/// From a catalog object, get procedures from all schemas.
		/// From a schema object get procedures from all this schema.
		/// From other object, return an empty list.
		///</summary>
		SharpQuerySchemaClassCollection GetSchemaProcedures(ISchemaClass schema);

		///<summary>
		/// From a table object, get columns from the table.
		/// From other object, return an empty list.
		///</summary>
		SharpQuerySchemaClassCollection GetSchemaTableColumns(ISchemaClass schema);

		///<summary>
		/// From a view object, get columns from the view.
		/// From other object, return an empty list.
		///</summary>
		SharpQuerySchemaClassCollection GetSchemaViewColumns(ISchemaClass schema);

		///<summary>
		/// From a procedure object , get columns from the procedure.
		/// From other object, return an empty list.
		///</summary>
		SharpQuerySchemaClassCollection GetSchemaProcedureColumns(ISchemaClass schema);

		///<summary>
		/// From a procedure object , get parameters from the procedure.
		/// From other object, return an empty list.
		///</summary>
		SharpQuerySchemaClassCollection GetSchemaProcedureParameters(ISchemaClass schema);
	}

}
