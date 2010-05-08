// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Luc Morin" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Data;
using System.Data.OleDb;

using SharpQuery.Collections;
using SharpQuery.Exceptions;
using SharpQuery.SchemaClass;

namespace SharpQuery.Connection
{
	//
	// ADO .NET Wrapper (better than OleDB)
	//
	///<summary>
	/// This class is associated with a class <see cref="ICSharpCode.SharpDevelop.Gui.SharpQuery.TreeView.SharpQueryNodeConnection"/>
	///</summary>
	public class OLEDBConnectionWrapper : AbstractSharpQueryConnectionWrapper
	{
		private System.Data.OleDb.OleDbConnection pOLEConnection = null;
		private System.Data.OleDb.OleDbDataAdapter pOLEAdapter = new OleDbDataAdapter();

		///<summary>
		///  OLEDB connection String.
		/// i use this for speed up the code writing ...
		///</summary>
		public override string ConnectionString
		{
			get
			{
				return base.ConnectionString;
			}

			set
			{
				if (IsOpen == true)
				{
					this.Close();
				}
				try
				{
					this.pOLEConnection.ConnectionString = value;
					wrongConnectionString = false;
				}
				catch (OleDbException e)
				{
					string mes = this.ConnectionString + "\n\r";
					wrongConnectionString = true;

					foreach (OleDbError err in e.Errors)
					{
						mes += "-----------------\n\r";
						mes += err.Message + "\n\r";
						mes += err.NativeError + "\n\r";
					}
					throw new ConnectionStringException(mes);
				}
				catch (System.Exception e)
				{
					wrongConnectionString = true;
					throw new ConnectionStringException(value
					                                    + "\n\r"
					                                    + "-------------------\n\r"
					                                    + e.Message
					                                   );
				}
			}
		}

		public override bool IsOpen
		{
			get
			{
				return (this.pOLEConnection.State == ConnectionState.Open);
			}
		}

		public override object GetProperty(AbstractSharpQueryConnectionWrapper.SharpQueryPropertyEnum property)
		{
			object returnValue = null;

			switch (property)
			{
				case SharpQueryPropertyEnum.Catalog:
				case SharpQueryPropertyEnum.DataSourceName:
					returnValue = this.pOLEConnection.Database;
					break;
				case SharpQueryPropertyEnum.ConnectionString:
					returnValue = this.pOLEConnection.ConnectionString.ToString();
					break;
				case SharpQueryPropertyEnum.DataSource:
					returnValue = this.pOLEConnection.DataSource;
					break;
				case SharpQueryPropertyEnum.DBMSName:
					returnValue = "";
					break;
				case SharpQueryPropertyEnum.ProviderName:
					//Key = "Provider Name";
					returnValue = this.pOLEConnection.Provider.ToString();
					break;
				default:
					returnValue = null;
					break;
			}

			return returnValue;
		}

		/// <summary>
		/// Creates a new DataConnection object
		/// </summary>
		public OLEDBConnectionWrapper()
			: base()
		{
			this.pOLEConnection = new System.Data.OleDb.OleDbConnection();
			this.pOLEAdapter = new System.Data.OleDb.OleDbDataAdapter();
			this.pOLEAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
		}

		public OLEDBConnectionWrapper(string connectionString)
			: this()
		{
			this.ConnectionString = connectionString;
		}

		public override bool Open()
		{
			try
			{
				if (this.IsOpen == false && wrongConnectionString == false)
				{
					this.pOLEConnection.Open();
				}
			}
			catch (OleDbException e)
			{
				string mes = this.ConnectionString + "\n\r";
				wrongConnectionString = true;

				foreach (OleDbError err in e.Errors)
				{
					mes += "-----------------\n\r";
					mes += err.Message + "\n\r";
					mes += err.NativeError + "\n\r";
				}
				throw new OpenConnectionException(mes);
			}
			catch (System.Exception)
			{
				wrongConnectionString = true;
				throw new OpenConnectionException(this.ConnectionString);
			}

			return this.IsOpen;
		}

		public override void Close()
		{
			if (this.IsOpen == true)
			{
				this.pOLEConnection.Close();
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
		/// </returns>		/// </summary>
		public override object ExecuteSQL(string SQLText, int rows)
		{
			OleDbCommand command = new OleDbCommand();
			DataSet returnValues = new DataSet();

			command.Connection = this.pOLEConnection;
			command.CommandText = SQLText;
			command.CommandType = System.Data.CommandType.Text;

			// some stranges things occurs with the OleDbDataAdapter and transaction
			command.Transaction = this.pOLEConnection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

			try
			{
				this.pOLEAdapter.SelectCommand = command;
				this.pOLEAdapter.Fill(returnValues);

			}
			catch (OleDbException e)
			{
				command.Transaction.Rollback();

				string mes = SQLText + "\n\r";

				foreach (OleDbError err in e.Errors)
				{
					mes += "-----------------\n\r";
					mes += err.Message + "\n\r";
					mes += err.NativeError + "\n\r";
				}
				throw new ExecuteSQLException(mes);
			}
			catch (System.Exception)
			{
				command.Transaction.Rollback();
				throw new ExecuteSQLException(SQLText);
			}
			finally
			{
				command.Transaction.Commit();
			}

			return returnValues;
		}

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
		/// </returns>		/// </summary>
		public override object ExecuteProcedure(ISchemaClass schema, int rows, SharpQuerySchemaClassCollection parameters)
		{

			DataSet returnValues = null;

			if (schema == null)
			{
				throw new System.ArgumentNullException("schema");
			}

			OleDbCommand command = new OleDbCommand();
			OleDbParameter para = null;
			returnValues = new DataSet();

			command.Connection = this.pOLEConnection;
			command.CommandText = schema.Name;
			command.CommandType = System.Data.CommandType.StoredProcedure;

			if (parameters != null)
			{
				foreach (SharpQueryParameter classParam in parameters)
				{
					para = new OleDbParameter();
					para.DbType = classParam.DataType;
					para.Direction = (ParameterDirection)classParam.Type;
					para.ParameterName = classParam.Name;
					if (para.ParameterName.StartsWith("["))
					{
						para.ParameterName = para.ParameterName.Remove(0, 1);
					}
					if (para.ParameterName.EndsWith("]"))
					{
						para.ParameterName = para.ParameterName.Remove(para.ParameterName.Length - 1, 1);
					}
					para.Value = classParam.Value;
					command.Parameters.Add(para);
				}
			}

			//				command.Prepare();
			command.Transaction = this.pOLEConnection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

			try
			{
				this.pOLEAdapter.SelectCommand = command;
				this.pOLEAdapter.Fill(returnValues);
			}
			catch (OleDbException e)
			{
				command.Transaction.Rollback();

				string mes = schema.Name + "\n\r";

				foreach (OleDbError err in e.Errors)
				{
					mes += "-----------------\n\r";
					mes += err.Message + "\n\r";
					mes += err.NativeError + "\n\r";
				}
				throw new ExecuteProcedureException(mes);
			}
			catch (System.Exception e)
			{
				command.Transaction.Rollback();
				throw new ExecuteProcedureException(e.Message);
			}

			command.Transaction.Commit();

			foreach (DataTable table in returnValues.Tables)
			{
				//readonly
				table.DefaultView.AllowDelete = false;
				table.DefaultView.AllowEdit = false;
				table.DefaultView.AllowNew = false;
			}

			return returnValues;
		}

		///<summary>
		/// check the internal connection object is valid
		/// Throw an exception if pADOConnection == null
		///</summary>
		protected override void CheckConnectionObject()
		{
			if (this.pOLEConnection == null)
				throw new ArgumentNullException("pOLEConnection");
		}

		protected override DataTable GetSchema(SharpQuerySchemaEnum schema, object[] restrictions)
		{
			System.Guid internalSchema = OleDbSchemaGuid.Assertions;
			DataTable OLErecord = null;

			switch (schema)
			{
				case SharpQuerySchemaEnum.Asserts:
					internalSchema = OleDbSchemaGuid.Assertions;
					break;
				case SharpQuerySchemaEnum.Catalogs:
					internalSchema = OleDbSchemaGuid.Catalogs;
					break;
				case SharpQuerySchemaEnum.CharacterSets:
					internalSchema = OleDbSchemaGuid.Character_Sets;
					break;
				case SharpQuerySchemaEnum.CheckConstraints:
					internalSchema = OleDbSchemaGuid.Check_Constraints;
					break;
				case SharpQuerySchemaEnum.Collations:
					internalSchema = OleDbSchemaGuid.Collations;
					break;
				case SharpQuerySchemaEnum.ColumnPrivileges:
					internalSchema = OleDbSchemaGuid.Column_Privileges;
					break;
				case SharpQuerySchemaEnum.Columns:
					internalSchema = OleDbSchemaGuid.Columns;
					break;
				case SharpQuerySchemaEnum.ConstraintColumnUsage:
					internalSchema = OleDbSchemaGuid.Constraint_Column_Usage;
					break;
				case SharpQuerySchemaEnum.ConstaintTableUsage:
					internalSchema = OleDbSchemaGuid.Constraint_Table_Usage;
					break;
				case SharpQuerySchemaEnum.DBInfoLiterals:
					internalSchema = OleDbSchemaGuid.DbInfoLiterals;
					break;
				case SharpQuerySchemaEnum.ForeignKeys:
					internalSchema = OleDbSchemaGuid.Foreign_Keys;
					break;
				case SharpQuerySchemaEnum.Indexes:
					internalSchema = OleDbSchemaGuid.Indexes;
					break;
				case SharpQuerySchemaEnum.KeyColumnUsage:
					internalSchema = OleDbSchemaGuid.Key_Column_Usage;
					break;
				case SharpQuerySchemaEnum.Null:
					break;
				case SharpQuerySchemaEnum.PrimaryKeys:
					internalSchema = OleDbSchemaGuid.Primary_Keys;
					break;
				case SharpQuerySchemaEnum.ProcedureColumns:
					internalSchema = OleDbSchemaGuid.Procedure_Columns;
					break;
				case SharpQuerySchemaEnum.ProcedureParameters:
					internalSchema = OleDbSchemaGuid.Procedure_Parameters;
					break;
				case SharpQuerySchemaEnum.Procedures:
					internalSchema = OleDbSchemaGuid.Procedures;
					break;
				case SharpQuerySchemaEnum.ProviderTypes:
					internalSchema = OleDbSchemaGuid.Provider_Types;
					break;
				case SharpQuerySchemaEnum.ReferentialConstraints:
					internalSchema = OleDbSchemaGuid.Referential_Constraints;
					break;
				case SharpQuerySchemaEnum.Schemata:
					internalSchema = OleDbSchemaGuid.Schemata;
					break;
				case SharpQuerySchemaEnum.SQLLanguages:
					internalSchema = OleDbSchemaGuid.Sql_Languages;
					break;
				case SharpQuerySchemaEnum.Statistics:
					internalSchema = OleDbSchemaGuid.Statistics;
					break;
				case SharpQuerySchemaEnum.TableConstraints:
					internalSchema = OleDbSchemaGuid.Table_Constraints;
					break;
				case SharpQuerySchemaEnum.TablePrivileges:
					internalSchema = OleDbSchemaGuid.Table_Privileges;
					break;
				case SharpQuerySchemaEnum.Tables:
				case SharpQuerySchemaEnum.Views:
					internalSchema = OleDbSchemaGuid.Tables;
					break;
				case SharpQuerySchemaEnum.Tanslations:
					internalSchema = OleDbSchemaGuid.Translations;
					break;
				case SharpQuerySchemaEnum.Trustees:
					internalSchema = OleDbSchemaGuid.Trustee;
					break;
				case SharpQuerySchemaEnum.UsagePrivileges:
					internalSchema = OleDbSchemaGuid.Usage_Privileges;
					break;
				case SharpQuerySchemaEnum.ViewColumnUsage:
					internalSchema = OleDbSchemaGuid.View_Column_Usage;
					break;
				case SharpQuerySchemaEnum.ViewColumns:
					internalSchema = OleDbSchemaGuid.Columns;
					break;
				case SharpQuerySchemaEnum.ViewTableUsage:
					internalSchema = OleDbSchemaGuid.View_Table_Usage;
					break;
				default:
					throw new System.ArgumentException("", "schema");
			}

			if (schema != SharpQuerySchemaEnum.Null)
			{
				OLErecord = this.pOLEConnection.GetOleDbSchemaTable(internalSchema, this.NormalizeRestrictions(restrictions));
			}



			return OLErecord;
		}

	}
}
