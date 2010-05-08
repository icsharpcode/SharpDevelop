// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Luc Morin" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using SharpQuery.Collections;
using SharpQuery.Connection;

namespace SharpQuery.SchemaClass
{
	public abstract class AbstractSharpQuerySchemaClass : ISchemaClass
	{
		protected string pCatalogName = null;
		protected string pSchemaName = null;
		protected string pOwnerName = null;
		protected string pName = null;

		protected IConnection pDataConnection = null;

		///<summary>
		/// check if there are white spaces into the string.
		/// if yes, then add "[" at the begin and "]" at the end.
		///</summary>
		internal static string CheckWhiteSpace(string str)
		{
			string returnStr = str;

			if (returnStr.IndexOf(" ") > -1)
			{
				if (returnStr.StartsWith("[") == false)
				{
					returnStr = "[" + returnStr;
				}
				if (returnStr.EndsWith("]") == false)
				{
					returnStr = returnStr + "]";
				}
			}

			return returnStr;
		}

		///<summary>remove "[" at the begin and at the end of the str</summary>
		internal static string RemoveBracket(string str)
		{
			string returnStr = str;
			if (returnStr.StartsWith("[") == true)
			{
				returnStr = returnStr.Remove(0, 1);
			}
			if (returnStr.EndsWith("]") == true)
			{
				returnStr = returnStr.Remove(returnStr.Length - 1, 1);
			}
			return returnStr;
		}


		///<summary>
		/// those, are list of the childs schema.
		/// i am using a dictionnary (<see cref="SharpQuery.Collections.SharpQueryListDictionary"></see>) because is more simplest to write
		/// <code>Entities["PROCEDURES"]</code> than <code>Entities[0]</code>.
		///</summary>
		protected SharpQueryListDictionary pEntities = null;

		public string CatalogName
		{
			get
			{
				return CheckWhiteSpace(this.pCatalogName);
			}
		}

		public string SchemaName
		{
			get
			{
				return CheckWhiteSpace(this.pSchemaName);
			}
		}

		public string OwnerName
		{
			get
			{
				return CheckWhiteSpace(this.pOwnerName);
			}
		}

		public string Name
		{
			get
			{
				return CheckWhiteSpace(this.pName);
			}
		}

		public string InternalName
		{
			get
			{
				return RemoveBracket(this.Name);
			}
		}

		public virtual string NormalizedName
		{
			get
			{
				return CheckWhiteSpace(Name);
			}
		}

		///<summary>
		/// those, are list of the childs schema.
		/// i am using a dictionnary (<see cref="SharpQuery.Collections.SharpQueryListDictionary"></see>) because is more simplest to write
		/// <code>Entities["PROCEDURES"]</code> than <code>Entities[0]</code>.
		///</summary>
		public SharpQueryListDictionary Entities
		{
			get
			{
				return pEntities;
			}
		}

		public IConnection Connection
		{
			get
			{
				return this.pDataConnection;
			}
		}

		///<summary> return a <see cref="System.Windows.Forms.DataObject">DataObject</see>
		///</summary>
		public virtual DataObject DragObject
		{
			get
			{
				return null;
			}
		}

		//create the entities list
		protected virtual void CreateEntitiesList()
		{
			if (this.pEntities == null)
			{
				this.pEntities = new SharpQueryListDictionary();
			}
		}

		///<summary>
		/// construtor
		/// <list type="bullet">
		///	<listheader>
		///		<term>parameters</term>
		///		<description></description>
		///	</listheader>
		///	<item>
		///		<term><code>connection</code></term>
		///		<description>connection object from wich this schema is extracted</description>
		///	</item>
		///	<item>
		///		<term><code>catalogName</code></term>
		///		<description> this is the catalog of this schema entity </description>
		///	</item>
		///	<item>
		///		<term><code>schemaName</code></term>
		///		<description> this is the schema of this schema entity </description>
		///	</item>
		///	<item>
		///		<term><code>ownerName</code></term>
		///		<description> this is the owner name of this schema entity </description>
		///	</item>
		///	<item>
		///		<term><code>name</code></term>
		///		<description> this is the name of this schema entity </description>
		///	</item>
		/// </list>
		///</summary>
		public AbstractSharpQuerySchemaClass(IConnection connection, string catalogName, string schemaName, string ownerName, string name)
			: base()
		{
			this.pCatalogName = catalogName;
			this.pSchemaName = schemaName;
			this.pOwnerName = ownerName;
			this.pName = name;
			this.pDataConnection = connection;

			this.CreateEntitiesList();
		}

		///<summary>
		/// called by <see cref=".Refresh()">Refresh</see> just after the <see cref=".Clear()">Clear</see> and before <see cref=".Refresh()">childs'refresh</see>.
		/// In this, you could change the <see cref=".Entities">Entities dicntionnary.</see>
		///</summary>
		protected abstract void OnRefresh();

		public void Refresh()
		{
			this.Clear();
			this.CreateEntitiesList();

			if (this.Connection.IsOpen == true)
			{
				this.OnRefresh();
			}
		}

		public void Clear()
		{
			if (this.pEntities != null)
			{
				this.pEntities.Clear();
			}
		}

		///<summary>
		/// For a Table or a View extract data.
		/// For a stocked procedure, execute it :o).
		/// <param name="rows">Number of row to extract. if "0", extract all rows.</param>
		/// <returns><see cref="System.Data.DataTable">DataTable</see>
		/// or a <see cref="System.Data.DataSet">DataSet</see> </returns>
		/// </summary>
		public abstract object Execute(int rows, SharpQuerySchemaClassCollection parameters);

		///<summary> if <see cref=".Dataconnection.CatalogName">CatalogName</see> is <code>null</code> or <code>empty</code>
		/// enumerate all catalogs from the database.
		/// Else enumerate the current catalog's properties.
		/// </summary>
		public virtual SharpQuerySchemaClassCollection GetSchemaCatalogs()
		{
			return this.pDataConnection.GetSchemaCatalogs(this);
		}

		///<summary> if <see cref=".Dataconnection.CatalogName">CatalogName</see> is <code>null</code> or <code>empty</code>
		/// enumerate all shcema from the database.
		/// Else enumerate schemas from the current catalog.
		/// </summary>
		public virtual SharpQuerySchemaClassCollection GetSchemaSchemas()
		{
			return this.pDataConnection.GetSchemaSchemas(this);
		}

		///<summary> Enumerate the <see cref=".CatalogName">CatalogName<see cref=".SchemaName">.SchemaName</see></see>'s tables
		/// </summary>
		public virtual SharpQuerySchemaClassCollection GetSchemaTables()
		{
			return this.pDataConnection.GetSchemaTables(this);
		}

		///<summary> Enumerate the <see cref=".CatalogName">CatalogName<see cref=".SchemaName">.SchemaName</see></see>'s views
		/// </summary>
		public virtual SharpQuerySchemaClassCollection GetSchemaViews()
		{
			return this.pDataConnection.GetSchemaViews(this);
		}

		///<summary> Enumerate the <see cref=".CatalogName">CatalogName<see cref=".SchemaName">.SchemaName</see></see>'s procedures
		/// </summary>
		public virtual SharpQuerySchemaClassCollection GetSchemaProcedures()
		{
			return this.pDataConnection.GetSchemaProcedures(this);
		}

		public virtual SharpQuerySchemaClassCollection GetSchemaColumns()
		{
			SharpQuerySchemaClassCollection list = new SharpQuerySchemaClassCollection();
			return list;
		}

		public virtual SharpQuerySchemaClassCollection GetSchemaParameters()
		{
			SharpQuerySchemaClassCollection list = new SharpQuerySchemaClassCollection();
			return list;
		}
	}
}
