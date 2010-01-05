// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Luc Morin" email=""/>
//     <version>$Revision$</version>
// </file>

using System.Windows.Forms;
using SharpQuery.Collections;
using SharpQuery.Connection;

namespace SharpQuery.SchemaClass
{
	///<summary>
	/// This interface is used by any schema class in sharp query.
	/// A schema class is a class that is the database definition of a <code>table</code>,
	/// a <code>view</code>, a <code>column</code>, etc etc ...
	/// <remarks>
	/// <code>entity</code> is <code>table</code>, a <code>view</code> or other database struct.
	/// <code>schema</code>'s names are displayed between <code>[</code> and <code>]</code>
	/// </remarks>
	///</summary>
	public interface ISchemaClass
	{
		///<summary>
		/// get the connection object
		///</summary>
		IConnection Connection { get; }

		///<summary>
		/// get the <code>entity</code>'s <code>catalog</code> name.
		///</summary>
		string CatalogName { get; }

		///<summary>
		/// get the <code>entity</code>'s <code>schema</code> name.
		///</summary>
		string SchemaName { get; }

		///<summary>
		/// Get the <code>entity</code>'s owner name
		/// <example>
		///  for exemple, the owner of a <code>column</code> is a <code>table</code>,
		/// and this property return the name of the <code>table</code>
		///</example>
		///</summary>
		string OwnerName { get; }

		///<summary>
		/// get the <code>entity</code>'s name
		///</summary>
		string Name { get; }

		/// <summary>
		/// Internal name, used internally
		/// </summary>
		string InternalName { get; }

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
		/// Those, are list of the childs schema.( columns, etc etc )
		/// i am using a dictionnary because is more simplest to write
		/// <code>Entities["PROCEDURES"]</code> than <code>Entities[0]</code>.
		///</summary>
		SharpQueryListDictionary Entities { get; }

		///<summary> return a <see cref="System.Windows.Forms.DataObject">DataObject</see> </summary>
		DataObject DragObject { get; }

		///<summary>
		/// Refresh child schemas from the database.
		///</summary>
		void Refresh();

		///<summary>
		/// Erase all child schemas
		///</summary>
		void Clear();

		///<summary>
		/// For a Table or a View extract data.
		/// For a stocked procedure, execute it :o).
		/// <param name="rows">Number of row to extract. if "0", extract all rows.</param>
		/// <returns><see cref="System.Data.DataTable">DataTable</see>
		/// or a <see cref="System.Data.DataSet">DataSet</see> </returns>
		/// </summary>
		object Execute(int rows, SharpQuerySchemaClassCollection parameters);

		///<summary>
		/// Get <seealso cref='.ISchemaClass.Connection'/>'s catalogs.
		///</summary>
		SharpQuerySchemaClassCollection GetSchemaCatalogs();

		///<summary>
		/// Get <seealso cref='.ISchemaClass.Connection'/>'s Schemas.
		///</summary>
		SharpQuerySchemaClassCollection GetSchemaSchemas();

		///<summary>
		/// From a catalog object, get tables from all schemas.
		/// From a schema object get tables from all this schema.
		/// From other object, return an empty list.
		///</summary>
		SharpQuerySchemaClassCollection GetSchemaTables();

		///<summary>
		/// From a catalog object, get views from all schemas.
		/// From a schema object get views from all this schema.
		/// From other object, return an empty list.
		///</summary>
		SharpQuerySchemaClassCollection GetSchemaViews();

		///<summary>
		/// From a catalog object, get procedures from all schemas.
		/// From a schema object get procedures from all this schema.
		/// From other object, return an empty list.
		///</summary>
		SharpQuerySchemaClassCollection GetSchemaProcedures();

		///<summary>
		/// From a table or a view or procedure object, get columns from the table.
		/// From other object, return an empty list.
		///</summary>
		SharpQuerySchemaClassCollection GetSchemaColumns();

		///<summary>
		/// Get parameters from the procedure.
		/// If parameters are not supported by the schema, return an empty list.
		///</summary>
		SharpQuerySchemaClassCollection GetSchemaParameters();
	}

}
