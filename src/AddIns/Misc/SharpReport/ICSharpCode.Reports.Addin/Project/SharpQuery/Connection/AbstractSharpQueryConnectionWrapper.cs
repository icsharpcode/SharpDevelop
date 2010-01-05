// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Luc Morin" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Data;

using ICSharpCode.Core;
using SharpQuery.Collections;
using SharpQuery.Exceptions;
using SharpQuery.SchemaClass;

namespace SharpQuery.Connection
{
	///<summary>
	/// this is a wrapper abstract class for connection with a database server.
	///</summary>
	public abstract class AbstractSharpQueryConnectionWrapper : IConnection
	{

		///<summary>
		/// SharpQuery schema enumeration.
		///</summary>
		public enum SharpQuerySchemaEnum
		{

			Asserts,
			Catalogs,
			CharacterSets,
			CheckConstraints,
			Collations,
			ColumnPrivileges,
			Columns,
			ColumnsDomainUsage,
			ConstraintColumnUsage,
			ConstaintTableUsage,
			Cubes,
			DBInfoKeyWords,
			DBInfoLiterals,
			Dimensions,
			ForeignKeys,
			Hierarchies,
			Indexes,
			KeyColumnUsage,
			Levels,
			Measures,
			Members,
			Null, // ask for an empty list
			PrimaryKeys,
			ProcedureColumns,
			ProcedureParameters,
			Procedures,
			Properties,
			ProviderSpecific,
			ProviderTypes,
			ReferentialConstraints,
			Schemata,
			SQLLanguages,
			Statistics,
			TableConstraints,
			TablePrivileges,
			Tables,
			Tanslations,
			Trustees,
			UsagePrivileges,
			ViewColumnUsage,
			Views,
			ViewColumns,
			ViewTableUsage
		}

		///<summary>
		/// Connection properties
		///</summary>
		public enum SharpQueryPropertyEnum
		{
			Catalog,
			ConnectionString,
			DataSource,
			DataSourceName,
			DBMSName,
			ProviderName
		}

		//constants
		internal string SELECT = "SELECT";
		internal string FROM = "FROM";
		internal string WHERE = "WHERE";
		internal string UPDATE = "UPDATE";
		internal string SET = "SET";
		internal string DELETE = "DELETE";
		internal string INSERINTO = "INSERT INTO";
		internal string VALUES = "VALUES";
		internal string AND = "AND";

		protected bool wrongConnectionString = false;
		protected SharpQueryListDictionary pEntities = null;


		///<summary>
		/// return <c>true</c> if the connection string is invalid.
		///</summary>
		public bool IsConnectionStringWrong
		{
			get
			{
				return this.wrongConnectionString;
			}
		}
		///<summary>return the catalog name. If there aren't a ctalog name
		/// in the <see cref=".ConnectionString">ConnectionString</see>, return "".
		/// </summary>
		public virtual string CatalogName
		{
			get
			{
				object returnValue = this.GetProperty(SharpQueryPropertyEnum.Catalog);

				if (returnValue == null)
				{
					returnValue = "";
				}

				return returnValue.ToString();
			}
		}

		public virtual string SchemaName
		{
			get
			{
				return "";//"INFORMATION_SCHEMA";
			}
		}

		public string Name
		{
			get
			{
				string Datasource;

				object returnValue = null;

				Datasource = this.GetProperty(SharpQueryPropertyEnum.DBMSName).ToString();
				if (Datasource == null)
				{
					Datasource = "";
				}

				if (Datasource != "")
				{
					returnValue += Datasource + ".";
				}

				Datasource = this.GetProperty(SharpQueryPropertyEnum.DataSource).ToString();
				if (Datasource == null)
				{
					Datasource = "";
				}

				if (Datasource != "")
				{
					returnValue += Datasource + ".";
				}

				if (this.CatalogName != "")
				{
					returnValue += this.CatalogName + ".";
				}

				return returnValue.ToString();
			}
		}

		///<summary>return  : <see cref=".Name">Name</see>.<see cref=".ConnectionString"></see></summary>
		public string NormalizedName
		{
			get
			{
				return this.Name + "." + this.ConnectionString;
			}
		}

		public SharpQueryListDictionary Entities
		{
			get
			{
				return this.pEntities;
			}
		}

		///<summary>
		///  OLEDB connection String.
		/// i use this for speed up the code writing ...
		///</summary>
		public virtual string ConnectionString
		{
			get
			{
				return this.GetProperty(SharpQueryPropertyEnum.ConnectionString).ToString();
			}

			set
			{
			}
		}

		public virtual string Provider
		{
			get
			{
				return this.GetProperty(SharpQueryPropertyEnum.ProviderName).ToString();
			}
		}

		public abstract bool IsOpen
		{
			get;
		}


		public abstract object GetProperty(AbstractSharpQueryConnectionWrapper.SharpQueryPropertyEnum property);

		/// <summary>
		/// Creates a new DataConnection object
		/// </summary>
		public AbstractSharpQueryConnectionWrapper()
		{
			this.pEntities = new SharpQueryListDictionary();
		}

		/// <summary>
		/// Creates a new DataConnection object from a connection string
		/// </summary>
		public AbstractSharpQueryConnectionWrapper(string connectionString)
			: this()
		{
		}

		static private IConnection CreateConnectionObject(string connectionstring)
		{
			try
			{
				AddInTreeNode AddinNode = AddInTree.GetTreeNode("/SharpQuery/Connection");
				IConnection conn = (IConnection)AddinNode.BuildChildItem("ConnectionWrapper", null, null);
				conn.ConnectionString = connectionstring;
				return conn;
			}
			catch (System.Exception e)
			{
				throw new ConnectionStringException(e.Message);
			}
		}

		static public IConnection CreateFromDataConnectionLink()
		{
			ADODB._Connection AdoConnection;
			MSDASC.DataLinks dataLink = new MSDASC.DataLinks();
			IConnection connection = null;

			AdoConnection = null;
			AdoConnection = (ADODB._Connection)dataLink.PromptNew();

			if ((AdoConnection != null) && (AdoConnection.ConnectionString != ""))
			{
				connection = CreateConnectionObject(AdoConnection.ConnectionString);
			}

			return connection;
		}

		static public IConnection UpDateFromDataConnectionLink(IConnection oldConnection)
		{
			object AdoConnection;
			MSDASC.DataLinks dataLink = new MSDASC.DataLinks();
			IConnection connection = null;

			AdoConnection = new ADODB.Connection();
			(AdoConnection as ADODB.Connection).ConnectionString = oldConnection.ConnectionString;

			if (dataLink.PromptEdit(ref AdoConnection))
			{
				connection = CreateConnectionObject((AdoConnection as ADODB.Connection).ConnectionString);
			}

			return connection;
		}

		static public IConnection CreateFromConnectionString(string stringConnection)
		{
			IConnection connection = null;

			if ((stringConnection != null) && (stringConnection != ""))
			{
				connection = CreateConnectionObject(stringConnection);
			}

			return connection;
		}

		public abstract bool Open();
		public abstract void Close();

		///<summary>
		/// called by <see cref=".Refresh()">Refresh</see> just after the <see cref=".Clear()">Clear</see> and before <see cref=".Refresh()">childs'refresh</see>.
		/// In this, you could change the <see cref=".Entities">Entities dicntionnary.</see>
		///</summary>
		protected virtual void OnRefresh()
		{
			SharpQuerySchemaClassCollection cl;

			if (this.pEntities != null)
			{
				cl = new SharpQuerySchemaClassCollection();
				cl.Add(new SharpQueryTables(this, this.CatalogName, this.SchemaName, this.Name, "TABLES"));
				this.pEntities.Add("TABLES", cl);

				cl = new SharpQuerySchemaClassCollection();
				cl.Add(new SharpQueryViews(this, this.CatalogName, this.SchemaName, this.Name, "VIEWS"));
				this.pEntities.Add("VIEWS", cl);

				cl = new SharpQuerySchemaClassCollection();
				cl.Add(new SharpQueryProcedures(this, this.CatalogName, this.SchemaName, this.Name, "PROCEDURES"));
				this.pEntities.Add("PROCEDURES", cl);
			}
		}

		///<summary>Refresh all dynamic properties of this connection</summary>
		public void Refresh()
		{
			this.Clear();

			if (this.IsOpen == true)
			{
				this.OnRefresh();
			}
		}

		public void Clear()
		{
			if (this.pEntities != null)
			{
				this.pEntities.Clear();

				//Let do the Garbage collector to clear the SharpQuerySchmaClassCollection childs.
				// It wil be do in a thread (by the garbage collector), it will be better
			}
		}

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
		public abstract object ExecuteSQL(string SQLText, int rows);
		//TODO : Parameter param.

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
		public abstract object ExecuteProcedure(ISchemaClass schema, int rows, SharpQuerySchemaClassCollection parameters);

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
		public object ExtractData(ISchemaClass schema, int rows)
		{

			if (schema == null)
			{
				throw new System.ArgumentNullException("schema");
			}

			string SQLSelect = this.SELECT + " ";
			string SQLFrom = this.FROM + " ";
			SharpQuerySchemaClassCollection entitieslist = null;

			SQLFrom += schema.Name;

			schema.Refresh();
			//we have only a table or view :o) 
			foreach (KeyValuePair<string, SharpQuerySchemaClassCollection> DicEntry in schema.Entities)
			{
				entitieslist = DicEntry.Value as SharpQuerySchemaClassCollection;
				break;
			}

			if (entitieslist == null)
			{
				throw new System.ArgumentNullException("entitieslist");
			}

			foreach (ISchemaClass column in entitieslist)
			{
				SQLSelect += column.NormalizedName;
				SQLSelect += ",";
			}

			SQLSelect = SQLSelect.TrimEnd(new Char[] { ',' });
			if (entitieslist.Count == 0)
			{
				SQLSelect += "*";
			}
			SQLSelect += " ";

			return this.ExecuteSQL(SQLSelect + SQLFrom, 0);
		}


		///<summary>
		/// Update <see cref="System.Data.DataRow">row</see>'s fields into the current opened database.
		/// <param name="row">a <see cref="System.Data.DataRow">row</see> </param>
		/// <param name="schema"> a <see cref="SharpQuery.SchemaClass.ISchema">schema</see> </param>
		/// <remarks> it use a transaction for each row, so it's a very long process
		/// if you should update something like 10 000 lines ;o). It's used only by the DataView.
		/// If you need a better way write a "BatchUpdate" function
		/// </remarks>
		///</summary>
		public void UpDateRow(ISchemaClass schema, DataRow row)
		{
			if (schema == null)
			{
				throw new System.ArgumentNullException("schema");
			}

			if (row == null)
			{
				throw new System.ArgumentNullException("row");
			}

			string SQLUpdate = this.UPDATE + " ";
			string SQLWhere = this.WHERE + " ";
			string SQLValues = this.SET + " ";

			SQLUpdate += schema.Name;
			SQLUpdate += " ";

			foreach (DataColumn column in row.Table.Columns)
			{
				if (column.ReadOnly == false
				    && column.AutoIncrement == false
				   )
				{
					SQLValues += schema.Name + "." + AbstractSharpQuerySchemaClass.CheckWhiteSpace(column.ColumnName);
					SQLValues += "=";
					if (column.DataType.Equals(System.Type.GetType("System.String"))
					    || column.DataType.Equals(System.Type.GetType("System.Char"))
					   )
					{
						SQLValues += "'";
					}
					SQLValues += row[column.ColumnName];
					if (column.DataType.Equals(System.Type.GetType("System.String"))
					    || column.DataType.Equals(System.Type.GetType("System.Char"))
					   )
					{
						SQLValues += "'";
					}

					SQLValues += ",";
				}

				SQLWhere += SharpQuery.SchemaClass.AbstractSharpQuerySchemaClass.CheckWhiteSpace(column.ColumnName);
				SQLWhere += "=";
				if (column.DataType.Equals(System.Type.GetType("System.String"))
				    || column.DataType.Equals(System.Type.GetType("System.Char"))
				   )
				{
					SQLWhere += "'";
				}
				SQLWhere += row[column.ColumnName, DataRowVersion.Original];
				if (column.DataType.Equals(System.Type.GetType("System.String"))
				    || column.DataType.Equals(System.Type.GetType("System.Char"))
				   )
				{
					SQLWhere += "'";
				}

				if (row.Table.Columns.IndexOf(column) != (row.Table.Columns.Count - 1))
				{
					SQLWhere += " " + this.AND + " ";
				}
			}

			SQLValues = SQLValues.TrimEnd(new Char[] { ',' });

			this.ExecuteSQL(SQLUpdate + SQLValues + SQLWhere, 0);
			row.AcceptChanges();
		}

		///<summary>
		/// Delete <see cref="System.Data.DataRow">row</see> into the current opened database.
		/// <param name="row">a <see cref="System.Data.DataRow">row</see> </param>
		/// <param name="schema"> a <see cref="SharpQuery.SchemaClass.ISchema">schema</see> </param>
		/// <remarks> it use a transaction for each row, so it's a very long process
		/// if you should update something like 10 000 lines ;o). It's used only by the DataView.
		/// If you need a better way write a "BatchUpdate" function
		/// </remarks>
		///</summary>
		public void DeleteRow(ISchemaClass schema, DataRow row)
		{
			if (schema == null)
			{
				throw new System.ArgumentNullException("schema");
			}

			if (row == null)
			{
				throw new System.ArgumentNullException("row");
			}

			string SQLDelete = this.DELETE + " ";
			string SQLWhere = this.WHERE + " ";
			string SQLFrom = this.FROM + " ";

			SQLFrom += schema.Name;
			SQLFrom += " ";

			foreach (DataColumn column in row.Table.Columns)
			{
				//SQLDelete += schema.Name + "." + column.ColumnName;

				SQLWhere += SharpQuery.SchemaClass.AbstractSharpQuerySchemaClass.CheckWhiteSpace(column.ColumnName);
				SQLWhere += "=";
				if (column.DataType.Equals(System.Type.GetType("System.String"))
				    || column.DataType.Equals(System.Type.GetType("System.Char"))
				   )
				{
					SQLWhere += "'";
				}
				SQLWhere += row[column.ColumnName, DataRowVersion.Original];
				if (column.DataType.Equals(System.Type.GetType("System.String"))
				    || column.DataType.Equals(System.Type.GetType("System.Char"))
				   )
				{
					SQLWhere += "'";
				}

				if (row.Table.Columns.IndexOf(column) != (row.Table.Columns.Count - 1))
				{
					//SQLDelete += ",";
					SQLWhere += " " + this.AND + " ";
				}
				else
				{
					//SQLDelete += " ";
				}
			}

			this.ExecuteSQL(SQLDelete + SQLFrom + SQLWhere, 0);
			row.AcceptChanges();
		}

		///<summary>
		/// Insert <see cref="System.Data.DataRow">row</see> into the current opened database.
		/// <param name="row">a <see cref="System.Data.DataRow">row</see> </param>
		/// <param name="schema"> a <see cref="SharpQuery.SchemaClass.ISchema">schema</see> </param>
		/// <remarks> it use a transaction for each row, so it's a very long process
		/// if you should update something like 10 000 lines ;o). It's used only by the DataView.
		/// If you need a better way write a "BatchUpdate" function
		/// </remarks>
		///</summary>
		public void InsertRow(ISchemaClass schema, DataRow row)
		{
			if (schema == null)
			{
				throw new System.ArgumentNullException("schema");
			}

			if (row == null)
			{
				throw new System.ArgumentNullException("row");
			}

			string SQLInsert = this.INSERINTO + " ";
			string SQLValues = this.VALUES + " (";

			SQLInsert += schema.Name;
			SQLInsert += " (";

			foreach (DataColumn column in row.Table.Columns)
			{
				if (column.ReadOnly == false
				    && column.AutoIncrement == false
				   )
				{
					SQLInsert += /*schema.Name + "." + //Full qualified name not supported by some provider*/ SharpQuery.SchemaClass.AbstractSharpQuerySchemaClass.CheckWhiteSpace(column.ColumnName);

					if (column.DataType.Equals(System.Type.GetType("System.String"))
					    || column.DataType.Equals(System.Type.GetType("System.Char"))
					   )
					{
						SQLValues += "'";
					}
					SQLValues += row[column.ColumnName, DataRowVersion.Current];
					if (column.DataType.Equals(System.Type.GetType("System.String"))
					    || column.DataType.Equals(System.Type.GetType("System.Char"))
					   )
					{
						SQLValues += "'";
					}

					SQLValues += ",";
					SQLInsert += ",";
				}
			}

			SQLValues = SQLValues.TrimEnd(new Char[] { ',' });
			SQLInsert = SQLInsert.TrimEnd(new Char[] { ',' });

			SQLInsert += ") ";
			SQLValues += ")";


			this.ExecuteSQL(SQLInsert + SQLValues, 0);
			row.AcceptChanges();
		}

		///<summary> throw a exception if the <seealso cref='.AbstractSharpQueryConnectionWrapper.Connection'/> is <code>null</code> </summary>
		protected abstract void CheckConnectionObject();

		///<summary> each elements of the restrictions array which are an empty string is replaced with a <code>null</code> reference</summary>
		protected object[] NormalizeRestrictions(object[] restrictions)
		{
			object[] newRestrictions = null;

			if (restrictions != null)
			{
				newRestrictions = new object[restrictions.Length];
				object restriction;

				for (int i = 0; i < restrictions.Length; i++)
				{
					restriction = restrictions[i];

					if (restriction != null)
					{
						if ((restriction is string) && ((restriction as string) == ""))
						{
							restriction = null;
						}
					}

					newRestrictions[i] = restriction;
				}
			}
			return newRestrictions;
		}

		/// <summary>
		/// return a schema matching <code>restrictions</code>
		/// <param name="schema"> a <see cref=".SharpQuerySchemaEnum">SharpQuerySchemaEnum</see>.</param>
		/// <param name="restrictions"> Restrictions matching the schema</param>
		/// </summary>
		protected abstract DataTable GetSchema(SharpQuerySchemaEnum schema, object[] restrictions);


		//
		// IConnection methods
		//

		public SharpQuerySchemaClassCollection GetSchemaCatalogs(ISchemaClass schema)
		{
			SharpQuerySchemaClassCollection list = new SharpQuerySchemaClassCollection();
			DataTable record = null;
			SharpQuerySchemaEnum schematype = SharpQuerySchemaEnum.Catalogs;
			object[] restrictions = new object[] { schema.InternalName };

			try
			{
				record = this.GetSchema(schematype, restrictions);

				//TODO : add not supported schema code!

				if (record != null)
				{
					foreach (DataRow row in record.Rows)
					{
						list.Add(new SharpQueryCatalog(this, row["CATALOG_NAME"].ToString(), "", "", row["CATALOG_NAME"].ToString()));
					}
				}
			}
			catch (System.Exception)
			{
				list.Add(new SharpQueryNotSupported(this, "", "", "", "SharpQuerySchemaEnum.Catalogs"));
			}

			return list;
		}

		public SharpQuerySchemaClassCollection GetSchemaSchemas(ISchemaClass schema)
		{
			SharpQuerySchemaClassCollection list = new SharpQuerySchemaClassCollection();
			DataTable record = null;
			SharpQuerySchemaEnum schematype = SharpQuerySchemaEnum.Schemata;
			object[] restrictions = new object[] { schema.CatalogName, "", "" };

			try
			{
				record = this.GetSchema(schematype, restrictions);

				if (record != null)
				{
					foreach (DataRow row in record.Rows)
					{
						list.Add(new SharpQuerySchema(this, row["CATALOG_NAME"].ToString(), row["SCHEMA_NAME"].ToString(), "", row["SCHEMA_NAME"].ToString()));
					}

				}
			}
			catch (System.Exception)
			{
				list.Add(new SharpQueryNotSupported(this, "", "", "", "SharpQuerySchemaEnum.Schemata"));
			}

			return list;
		}

		public SharpQuerySchemaClassCollection GetSchemaTables(ISchemaClass schema)
		{
			SharpQuerySchemaClassCollection list = new SharpQuerySchemaClassCollection();
			DataTable record = null;
			SharpQuerySchemaEnum schematype = SharpQuerySchemaEnum.Tables;
			object[] restrictions = new object[] { schema.CatalogName, schema.SchemaName, "", "TABLE" };

			try
			{
				record = this.GetSchema(schematype, restrictions);

				if (record != null)
				{
					foreach (DataRow row in record.Rows)
					{
						list.Add(new SharpQueryTable(this, row["TABLE_CATALOG"].ToString(), row["TABLE_SCHEMA"].ToString(), "", row["TABLE_NAME"].ToString()));
					}
				}
			}
			catch (System.Exception)
			{
				list.Add(new SharpQueryNotSupported(this, "", "", "", "SharpQuerySchemaEnum.Tables"));
			}

			return list;
		}

		public SharpQuerySchemaClassCollection GetSchemaViews(ISchemaClass schema)
		{
			SharpQuerySchemaClassCollection list = new SharpQuerySchemaClassCollection();
			DataTable record = null;
			SharpQuerySchemaEnum schematype = SharpQuerySchemaEnum.Views;
			object[] restrictions = new object[] { schema.CatalogName, schema.SchemaName, "", "VIEW" };

			try
			{
				record = this.GetSchema(schematype, restrictions);

				if (record != null)
				{
					foreach (DataRow row in record.Rows)
					{
						list.Add(new SharpQueryView(this, row["TABLE_CATALOG"].ToString(), row["TABLE_SCHEMA"].ToString(), "", row["TABLE_NAME"].ToString()));
					}
				}
			}
			catch (System.Exception)
			{
				list.Add(new SharpQueryNotSupported(this, "", "", "", "SharpQuerySchemaEnum.Views"));
			}

			return list;
		}

		public SharpQuerySchemaClassCollection GetSchemaProcedures(ISchemaClass schema)
		{
			SharpQuerySchemaClassCollection list = new SharpQuerySchemaClassCollection();
			DataTable record = null;
			SharpQuerySchemaEnum schematype = SharpQuerySchemaEnum.Procedures;
			object[] restrictions = new object[] { schema.CatalogName, schema.SchemaName, "", "" };

			try
			{
				record = this.GetSchema(schematype, restrictions);

				if (record != null)
				{
					foreach (DataRow row in record.Rows)
					{
						list.Add(new SharpQueryProcedure(this, row["PROCEDURE_CATALOG"].ToString(), row["PROCEDURE_SCHEMA"].ToString(), "", row["PROCEDURE_NAME"].ToString().Split(';')[0]));
					}
				}
			}
			catch (System.Exception)
			{
				list.Add(new SharpQueryNotSupported(this, "", "", "", "SharpQuerySchemaEnum.Procedures"));
			}

			return list;
		}

		public SharpQuerySchemaClassCollection GetSchemaTableColumns(ISchemaClass schema)
		{
			SharpQuerySchemaClassCollection list = new SharpQuerySchemaClassCollection();
			DataTable record = null;
			SharpQuerySchemaEnum schematype = SharpQuerySchemaEnum.Columns;
			object[] restrictions = new object[] { schema.CatalogName, schema.SchemaName, schema.InternalName, "" };

			try
			{
				record = this.GetSchema(schematype, restrictions);

				if (record != null)
				{
					foreach (DataRow row in record.Rows)
					{
						list.Add(new SharpQueryColumn(this, schema.CatalogName, schema.SchemaName, schema.Name, row["COLUMN_NAME"].ToString()));
					}
				}
			}
			catch (System.Exception)
			{
				list.Add(new SharpQueryNotSupported(this, "", "", "", "SharpQuerySchemaEnum.Columns"));
			}

			return list;
		}

		public SharpQuerySchemaClassCollection GetSchemaViewColumns(ISchemaClass schema)
		{
			SharpQuerySchemaClassCollection list = new SharpQuerySchemaClassCollection();
			DataTable record = null;
			SharpQuerySchemaEnum schematype = SharpQuerySchemaEnum.ViewColumns;
			object[] restrictions = new object[] { schema.CatalogName, schema.SchemaName, schema.InternalName, "" };

			try
			{
				record = this.GetSchema(schematype, restrictions);

				if (record != null)
				{
					foreach (DataRow row in record.Rows)
					{
						list.Add(new SharpQueryColumn(this, schema.CatalogName, schema.SchemaName, schema.Name, row["COLUMN_NAME"].ToString()));
					}
				}
			}
			catch (System.Exception)
			{
				list.Add(new SharpQueryNotSupported(this, "", "", "", "SharpQuerySchemaEnum.Columns"));
			}

			return list;
		}

		public SharpQuerySchemaClassCollection GetSchemaProcedureColumns(ISchemaClass schema)
		{
			SharpQuerySchemaClassCollection list = new SharpQuerySchemaClassCollection();
			DataTable record = null;
			SharpQuerySchemaEnum schematype = SharpQuerySchemaEnum.ProcedureColumns;
			object[] restrictions = new object[] { schema.CatalogName, schema.SchemaName, schema.InternalName, "" };

			try
			{
				record = this.GetSchema(schematype, restrictions);

				if (record != null)
				{
					foreach (DataRow row in record.Rows)
					{
						list.Add(new SharpQueryColumn(this, schema.CatalogName, schema.SchemaName, schema.Name, row["COLUMN_NAME"].ToString()));
					}
				}
			}
			catch (System.Exception)
			{
				list.Add(new SharpQueryNotSupported(this, "", "", "", "SharpQuerySchemaEnum.ProcedureColumns"));
			}

			return list;
		}

		public SharpQuerySchemaClassCollection GetSchemaProcedureParameters(ISchemaClass schema)
		{
			SharpQuerySchemaClassCollection list = new SharpQuerySchemaClassCollection();
			DataTable record = null;
			SharpQuerySchemaEnum schematype = SharpQuerySchemaEnum.ProcedureParameters;
			object[] restrictions = new object[] { schema.CatalogName, schema.SchemaName, schema.InternalName, "" };

			try
			{
				record = this.GetSchema(schematype, restrictions);
				SharpQueryParameter par = null;
				if (record != null)
				{
					foreach (DataRow row in record.Rows)
					{
						par = new SharpQueryParameter(this, schema.CatalogName, schema.SchemaName, schema.Name, row["PARAMETER_NAME"].ToString());
						par.DataType = StringToDbType(row["DATA_TYPE"].ToString());
						par.Type = StringToParamDirection(row["PARAMETER_TYPE"].ToString());

						if (par.Type != ParameterDirection.ReturnValue)
						{
							list.Add(par);
						}
					}
				}
			}
			catch (System.Exception)
			{
				list.Add(new SharpQueryNotSupported(this, "", "", "", "SharpQuerySchemaEnum.ProcedureParameters"));
			}

			return list;
		}


		protected DbType StringToDbType(string value)
		{
			return IntToDbType(int.Parse(value));
		}

		protected DbType IntToDbType(int value)
		{
			DbType retValue;
			switch (value)
			{
					case 129: retValue = DbType.AnsiString; break;
					//case 1	: retValue = DbType.AnsiStringFixedLength; break;
					case 128: retValue = DbType.Binary; break;
					case 11: retValue = DbType.Boolean; break;
					case 17: retValue = DbType.Byte; break;
					case 6: retValue = DbType.Currency; break;
				case 7:
					case 133: retValue = DbType.Date; break;
					case 135: retValue = DbType.DateTime; break;
					case 14: retValue = DbType.Decimal; break;
					case 5: retValue = DbType.Double; break;
					case 72: retValue = DbType.Guid; break;
					case 2: retValue = DbType.Int16; break;
					case 3: retValue = DbType.Int32; break;
					case 20: retValue = DbType.Int64; break;
				case 12:
					case 132: retValue = DbType.Object; break;
					case 16: retValue = DbType.SByte; break;
					case 4: retValue = DbType.Single; break;
					case 130: retValue = DbType.String; break;
					case 8: retValue = DbType.StringFixedLength; break;
					case 134: retValue = DbType.Time; break;
					case 18: retValue = DbType.UInt16; break;
					case 19: retValue = DbType.UInt32; break;
					case 21: retValue = DbType.UInt64; break;
					case 131: retValue = DbType.VarNumeric; break;
					default: throw new ArgumentOutOfRangeException("value");
			}

			return retValue;
		}

		protected ParameterDirection StringToParamDirection(string value)
		{
			return IntToParamDirection(int.Parse(value));
		}

		protected ParameterDirection IntToParamDirection(int value)
		{
			ParameterDirection retValue;
			switch (value)
			{
					case 1: retValue = ParameterDirection.Input; break;
					case 2: retValue = ParameterDirection.InputOutput; break;
					case 3: retValue = ParameterDirection.Output; break;
					case 4: retValue = ParameterDirection.ReturnValue; break;
					default: throw new ArgumentOutOfRangeException("value");

			}

			return retValue;
		}

	}
}
