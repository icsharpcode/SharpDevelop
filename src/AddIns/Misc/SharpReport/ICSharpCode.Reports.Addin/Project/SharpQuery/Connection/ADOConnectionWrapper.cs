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
	public class ADOConnectionWrapper : AbstractSharpQueryConnectionWrapper
	{
		protected ADODB.ConnectionClass pADOConnection;

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
					this.pADOConnection.ConnectionString = value;
					wrongConnectionString = false;
				}
				catch (System.Exception)
				{
					string mes = this.ConnectionString + "\n\r";
					wrongConnectionString = true;

					foreach (ADODB.Error err in this.pADOConnection.Errors)
					{
						mes += "-----------------\n\r";
						mes += err.Description + "\n\r";
						mes += err.NativeError + "\n\r";
					}
					throw new ConnectionStringException(mes);
				}
			}
		}

		public override bool IsOpen
		{
			get
			{
				return (pADOConnection.State == (int)ADODB.ObjectStateEnum.adStateOpen);
			}
		}

		public override object GetProperty(AbstractSharpQueryConnectionWrapper.SharpQueryPropertyEnum property)
		{
			object returnValue = null;
			string Key = null;

			switch (property)
			{
				case SharpQueryPropertyEnum.Catalog:
					Key = "Current Catalog";
					break;
				case SharpQueryPropertyEnum.ConnectionString:
					returnValue = this.pADOConnection.ConnectionString.ToString();
					break;
				case SharpQueryPropertyEnum.DataSource:
					Key = "Data Source";
					break;
				case SharpQueryPropertyEnum.DataSourceName:
					Key = "Data Source Name";
					break;
				case SharpQueryPropertyEnum.DBMSName:
					Key = "DBMS Name";
					break;
				case SharpQueryPropertyEnum.ProviderName:
					returnValue = this.pADOConnection.Provider.ToString();
					break;
				default:
					Key = null;
					break;
			}

			try
			{
				if (Key != null)
				{
					if (this.pADOConnection.Properties[Key].Value != null)
					{
						returnValue = this.pADOConnection.Properties[Key].Value;
					}
				}
			}
			catch (System.Exception)
			{
				returnValue = null;
			}

			return returnValue;
		}

		/// <summary>
		/// Creates a new DataConnection object
		/// </summary>
		public ADOConnectionWrapper()
			: base()
		{
			this.pADOConnection = new ADODB.ConnectionClass();
		}

		public ADOConnectionWrapper(string connectionString)
			: this()
		{
			this.ConnectionString = connectionString;
		}

		protected override void OnRefresh()
		{
			this.pADOConnection.Properties.Refresh();

			base.OnRefresh();
		}

		public override bool Open()
		{
			try
			{
				if (this.IsOpen == false && wrongConnectionString == false)
				{
					this.pADOConnection.GetType().InvokeMember(
					                                           "Open",
					                                           System.Reflection.BindingFlags.InvokeMethod,
					                                           null,
					                                           this.pADOConnection,
					                                           null);
				}
			}
			catch (System.Exception)
			{
				string mes = this.ConnectionString + "\n\r";
				wrongConnectionString = true;

				foreach (ADODB.Error err in this.pADOConnection.Errors)
				{
					mes += "-----------------\n\r";
					mes += err.Description + "\n\r";
					mes += err.NativeError + "\n\r";
				}
				throw new OpenConnectionException(mes);
			}

			return this.IsOpen;
		}

		public override void Close()
		{
			if (this.IsOpen == true)
			{
				this.pADOConnection.Close();
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
		public override object ExecuteSQL(string SQLText, int rows)
		{
			ADODB.Recordset record = new ADODB.Recordset();
			DataTable table = null;

			this.pADOConnection.BeginTrans();

			try
			{
				record.MaxRecords = rows;
				record.Open((object)SQLText,
				            (object)this.pADOConnection,
				            ADODB.CursorTypeEnum.adOpenDynamic,
				            ADODB.LockTypeEnum.adLockPessimistic,
				            (int)ADODB.CommandTypeEnum.adCmdText
				           );

				table = RecordSetToDataTable(record);
			}
			catch (System.Exception)
			{
				this.pADOConnection.RollbackTrans();

				string mes = SQLText + "\n\r";

				foreach (ADODB.Error err in this.pADOConnection.Errors)
				{
					mes += "-----------------\n\r";
					mes += err.Description + "\n\r";
					mes += err.NativeError + "\n\r";
				}
				throw new ExecuteSQLException(mes);
			}
			finally
			{
				this.pADOConnection.CommitTrans();
			}

			return table;
		}


		protected ADODB.ParameterDirectionEnum ParamDirectionToADODirection(ParameterDirection dir)
		{
			ADODB.ParameterDirectionEnum ret = ADODB.ParameterDirectionEnum.adParamInput;

			switch (dir)
			{
					case ParameterDirection.Input: ret = ADODB.ParameterDirectionEnum.adParamInput; break;
					case ParameterDirection.InputOutput: ret = ADODB.ParameterDirectionEnum.adParamInputOutput; break;
					case ParameterDirection.Output: ret = ADODB.ParameterDirectionEnum.adParamOutput; break;
					case ParameterDirection.ReturnValue: ret = ADODB.ParameterDirectionEnum.adParamReturnValue; break;
					default: throw new ArgumentOutOfRangeException("dir");
			}

			return ret;
		}


		protected ADODB.DataTypeEnum DbTypeToDataType(DbType t)
		{
			ADODB.DataTypeEnum ret = ADODB.DataTypeEnum.adChar;

			switch (t)
			{
					case DbType.AnsiString: ret = ADODB.DataTypeEnum.adChar; break;
					case DbType.Binary: ret = ADODB.DataTypeEnum.adBinary; break;
					case DbType.Boolean: ret = ADODB.DataTypeEnum.adBoolean; break;
					case DbType.Byte: ret = ADODB.DataTypeEnum.adUnsignedTinyInt; break;
					case DbType.Currency: ret = ADODB.DataTypeEnum.adCurrency; break;
					case DbType.Date: ret = ADODB.DataTypeEnum.adDate; break;
					case DbType.DateTime: ret = ADODB.DataTypeEnum.adDBTimeStamp; break;
					case DbType.Decimal: ret = ADODB.DataTypeEnum.adDecimal; break;
					case DbType.Double: ret = ADODB.DataTypeEnum.adDouble; break;
					case DbType.Guid: ret = ADODB.DataTypeEnum.adGUID; break;
					case DbType.Int16: ret = ADODB.DataTypeEnum.adSmallInt; break;
					case DbType.Int32: ret = ADODB.DataTypeEnum.adInteger; break;
					case DbType.Int64: ret = ADODB.DataTypeEnum.adBigInt; break;
					case DbType.Object: ret = ADODB.DataTypeEnum.adUserDefined; break;
					case DbType.SByte: ret = ADODB.DataTypeEnum.adTinyInt; break;
					case DbType.Single: ret = ADODB.DataTypeEnum.adSingle; break;
					case DbType.String: ret = ADODB.DataTypeEnum.adVarWChar; break;
					case DbType.StringFixedLength: ret = ADODB.DataTypeEnum.adWChar; break;
					case DbType.Time: ret = ADODB.DataTypeEnum.adDBTime; break;
					case DbType.UInt16: ret = ADODB.DataTypeEnum.adUnsignedSmallInt; break;
					case DbType.UInt32: ret = ADODB.DataTypeEnum.adUnsignedInt; break;
					case DbType.UInt64: ret = ADODB.DataTypeEnum.adUnsignedBigInt; break;
					case DbType.VarNumeric: ret = ADODB.DataTypeEnum.adVarNumeric; break;
					default: throw new ArgumentOutOfRangeException("t");
			}

			return ret;
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
		/// </returns>
		/// </summary>
		public override object ExecuteProcedure(ISchemaClass schema, int rows, SharpQuerySchemaClassCollection parameters)
		{
			DataTable table = null;

			if (schema == null)
			{
				throw new System.ArgumentNullException("schema");
			}

			ADODB.Recordset record = null;
			ADODB.Command command = new ADODB.Command();
			command.ActiveConnection = this.pADOConnection;
			ADODB.Parameter para = null;

			command.CommandText = schema.Name;
			command.CommandType = ADODB.CommandTypeEnum.adCmdStoredProc;

			if (parameters != null)
			{
				foreach (SharpQueryParameter classParam in parameters)
				{
					para = new ADODB.Parameter();
					para.Type = DbTypeToDataType(classParam.DataType);
					para.Direction = ParamDirectionToADODirection(classParam.Type);
					para.Name = classParam.Name;
					if (para.Name.StartsWith("["))
					{
						para.Name = para.Name.Remove(0, 1);
					}
					if (para.Name.EndsWith("]"))
					{
						para.Name = para.Name.Remove(para.Name.Length - 1, 1);
					}
					para.Value = classParam.Value;
					command.Parameters.Append(para);
				}
			}

			this.pADOConnection.BeginTrans();

			try
			{
				record = (ADODB.Recordset)command.GetType().InvokeMember(
				                                                         "Execute",
				                                                         System.Reflection.BindingFlags.InvokeMethod,
				                                                         null,
				                                                         command,
				                                                         null);

				//record.MaxRecords = rows;
				table = RecordSetToDataTable(record);

				//Procedure is ReadOnly
				table.DefaultView.AllowDelete = false;
				table.DefaultView.AllowEdit = false;
				table.DefaultView.AllowNew = false;
			}
			catch (System.Exception e)
			{
				if (schema != null)
				{
					this.pADOConnection.RollbackTrans();

					string mes = schema.Name + "\n\r";

					foreach (ADODB.Error err in this.pADOConnection.Errors)
					{
						mes += "-----------------\n\r";
						mes += err.Description + "\n\r";
						mes += err.NativeError + "\n\r";
					}
					throw new ExecuteProcedureException(mes);
				}
				else
				{
					throw new ExecuteProcedureException(e.Message);
				}
			}

			this.pADOConnection.CommitTrans();

			return table;
		}

		///<summary>
		/// check the internal connection object is valid
		/// Throw an exception if pADOConnection == null
		///</summary>
		protected override void CheckConnectionObject()
		{
			if (this.pADOConnection == null)
				throw new ArgumentNullException("pADONETConnection");
		}

		protected DataTable RecordSetToDataTable(ADODB.Recordset record)
		{
			OleDbDataAdapter custDA = new OleDbDataAdapter();
			DataTable custTable = new DataTable();
			custDA.Fill(custTable, record);
			return custTable;
		}

		protected override DataTable GetSchema(SharpQuerySchemaEnum schema, object[] restrictions)
		{
			ADODB.SchemaEnum internalSchema = 0;
			ADODB.Recordset ADOrecord = null;

			switch (schema)
			{
				case SharpQuerySchemaEnum.Asserts:
					internalSchema = ADODB.SchemaEnum.adSchemaAsserts;
					break;
				case SharpQuerySchemaEnum.Catalogs:
					internalSchema = ADODB.SchemaEnum.adSchemaCatalogs;
					break;
				case SharpQuerySchemaEnum.CharacterSets:
					internalSchema = ADODB.SchemaEnum.adSchemaCharacterSets;
					break;
				case SharpQuerySchemaEnum.CheckConstraints:
					internalSchema = ADODB.SchemaEnum.adSchemaCheckConstraints;
					break;
				case SharpQuerySchemaEnum.Collations:
					internalSchema = ADODB.SchemaEnum.adSchemaCollations;
					break;
				case SharpQuerySchemaEnum.ColumnPrivileges:
					internalSchema = ADODB.SchemaEnum.adSchemaColumnPrivileges;
					break;
				case SharpQuerySchemaEnum.Columns:
					internalSchema = ADODB.SchemaEnum.adSchemaColumns;
					break;
				case SharpQuerySchemaEnum.ColumnsDomainUsage:
					internalSchema = ADODB.SchemaEnum.adSchemaColumnsDomainUsage;
					break;
				case SharpQuerySchemaEnum.ConstraintColumnUsage:
					internalSchema = ADODB.SchemaEnum.adSchemaConstraintColumnUsage;
					break;
				case SharpQuerySchemaEnum.ConstaintTableUsage:
					internalSchema = ADODB.SchemaEnum.adSchemaConstraintTableUsage;
					break;
				case SharpQuerySchemaEnum.Cubes:
					internalSchema = ADODB.SchemaEnum.adSchemaCubes;
					break;
				case SharpQuerySchemaEnum.DBInfoKeyWords:
					internalSchema = ADODB.SchemaEnum.adSchemaDBInfoKeywords;
					break;
				case SharpQuerySchemaEnum.DBInfoLiterals:
					internalSchema = ADODB.SchemaEnum.adSchemaDBInfoLiterals;
					break;
				case SharpQuerySchemaEnum.Dimensions:
					internalSchema = ADODB.SchemaEnum.adSchemaDimensions;
					break;
				case SharpQuerySchemaEnum.ForeignKeys:
					internalSchema = ADODB.SchemaEnum.adSchemaForeignKeys;
					break;
				case SharpQuerySchemaEnum.Hierarchies:
					internalSchema = ADODB.SchemaEnum.adSchemaHierarchies;
					break;
				case SharpQuerySchemaEnum.Indexes:
					internalSchema = ADODB.SchemaEnum.adSchemaIndexes;
					break;
				case SharpQuerySchemaEnum.KeyColumnUsage:
					internalSchema = ADODB.SchemaEnum.adSchemaKeyColumnUsage;
					break;
				case SharpQuerySchemaEnum.Levels:
					internalSchema = ADODB.SchemaEnum.adSchemaLevels;
					break;
				case SharpQuerySchemaEnum.Measures:
					internalSchema = ADODB.SchemaEnum.adSchemaMeasures;
					break;
				case SharpQuerySchemaEnum.Members:
					internalSchema = ADODB.SchemaEnum.adSchemaMembers;
					break;
				case SharpQuerySchemaEnum.Null:
					break;
				case SharpQuerySchemaEnum.PrimaryKeys:
					internalSchema = ADODB.SchemaEnum.adSchemaPrimaryKeys;
					break;
				case SharpQuerySchemaEnum.ProcedureColumns:
					internalSchema = ADODB.SchemaEnum.adSchemaProcedureColumns;
					break;
				case SharpQuerySchemaEnum.ProcedureParameters:
					internalSchema = ADODB.SchemaEnum.adSchemaProcedureParameters;
					break;
				case SharpQuerySchemaEnum.Procedures:
					internalSchema = ADODB.SchemaEnum.adSchemaProcedures;
					break;
				case SharpQuerySchemaEnum.Properties:
					internalSchema = ADODB.SchemaEnum.adSchemaProperties;
					break;
				case SharpQuerySchemaEnum.ProviderTypes:
					internalSchema = ADODB.SchemaEnum.adSchemaProviderTypes;
					break;
				case SharpQuerySchemaEnum.ReferentialConstraints:
					internalSchema = ADODB.SchemaEnum.adSchemaReferentialConstraints;
					break;
				case SharpQuerySchemaEnum.Schemata:
					internalSchema = ADODB.SchemaEnum.adSchemaSchemata;
					break;
				case SharpQuerySchemaEnum.SQLLanguages:
					internalSchema = ADODB.SchemaEnum.adSchemaSQLLanguages;
					break;
				case SharpQuerySchemaEnum.Statistics:
					internalSchema = ADODB.SchemaEnum.adSchemaStatistics;
					break;
				case SharpQuerySchemaEnum.TableConstraints:
					internalSchema = ADODB.SchemaEnum.adSchemaTableConstraints;
					break;
				case SharpQuerySchemaEnum.TablePrivileges:
					internalSchema = ADODB.SchemaEnum.adSchemaTablePrivileges;
					break;
				case SharpQuerySchemaEnum.Tables:
				case SharpQuerySchemaEnum.Views:
					internalSchema = ADODB.SchemaEnum.adSchemaTables;
					break;
				case SharpQuerySchemaEnum.Tanslations:
					internalSchema = ADODB.SchemaEnum.adSchemaTranslations;
					break;
				case SharpQuerySchemaEnum.Trustees:
					internalSchema = ADODB.SchemaEnum.adSchemaTrustees;
					break;
				case SharpQuerySchemaEnum.UsagePrivileges:
					internalSchema = ADODB.SchemaEnum.adSchemaUsagePrivileges;
					break;
				case SharpQuerySchemaEnum.ViewColumnUsage:
					internalSchema = ADODB.SchemaEnum.adSchemaViewColumnUsage;
					break;
				case SharpQuerySchemaEnum.ViewColumns:
					internalSchema = ADODB.SchemaEnum.adSchemaColumns;
					break;
				case SharpQuerySchemaEnum.ViewTableUsage:
					internalSchema = ADODB.SchemaEnum.adSchemaViewTableUsage;
					break;
				default:
					throw new System.ArgumentException("", "schema");
			}

			if (schema != SharpQuerySchemaEnum.Null)
			{
				ADOrecord = (ADODB.Recordset)this.pADOConnection.GetType().InvokeMember(
				                                                                        "OpenSchema",
				                                                                        System.Reflection.BindingFlags.InvokeMethod,
				                                                                        null,
				                                                                        this.pADOConnection,
				                                                                        new Object[] { internalSchema, this.NormalizeRestrictions(restrictions) });
			}

			return RecordSetToDataTable(ADOrecord);
		}

	}
}
