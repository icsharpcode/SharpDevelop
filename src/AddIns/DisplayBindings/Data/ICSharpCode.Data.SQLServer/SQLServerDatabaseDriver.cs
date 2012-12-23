// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Sql;
using ICSharpCode.Data.Core.Common;
using ICSharpCode.Data.Core.Interfaces;
using System.Collections.ObjectModel;
using ICSharpCode.Data.Core.DatabaseObjects;
using System.Data.SqlClient;
using System.Collections.Specialized;
using ICSharpCode.Data.Core.Enums;
using System.Windows;
using System.Windows.Threading;

#endregion

namespace ICSharpCode.Data.Core.DatabaseDrivers.SQLServer
{
    public class SQLServerDatabaseDriver : DatabaseDriver<SQLServerDatasource>
    {
        #region Consts
        
        private const string _getTables = @"SELECT TABLE_SCHEMA, TABLE_NAME from INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME<>'dtproperties' ORDER BY TABLE_SCHEMA, TABLE_NAME";

        //http://community.sharpdevelop.net/forums/p/12955/37213.aspx#37213
        //remove this line clmns.is_column_set AS [IsColumnSet], 
        
        private const string _getColumnsScript = @"DECLARE @tablename varchar(100) SET @tablename = N'{0}'
            SELECT
                clmns.column_id AS [ColumnId],
                clmns.name AS [Name],
                usrt.name AS [DataType],
                ISNULL(baset.name, N'') AS [SystemType],
                CAST(CASE WHEN baset.name IN (N'nchar', N'nvarchar') AND clmns.max_length <> -1 THEN
                clmns.max_length/2 ELSE 0 END AS INT) AS [Length],
                CAST(clmns.precision AS int) AS [NumericPrecision],
                clmns.default_object_id AS [DefaultObjectId],
                clmns.is_ansi_padded AS [IsAnsiPadded],
             
                clmns.is_computed AS [IsComputed],
                clmns.is_dts_replicated AS [IsDtsReplicated],
                clmns.is_filestream AS [IsFileStream],
                clmns.is_identity AS [IsIdentity],
                clmns.is_merge_published AS [IsMergePublished],
                clmns.is_non_sql_subscribed AS [IsNonSqlSubscribed],
                clmns.is_nullable AS [IsNullable],
                clmns.is_replicated AS [IsReplicated],
                clmns.is_rowguidcol AS [IsRowGuidCol],
                clmns.is_sparse AS [IsSparse],
                clmns.is_xml_document AS [IsXmlDocument],
                clmns.object_id AS [ObjectId],
                clmns.rule_object_id AS [RuleObjectId],
                clmns.scale AS [Scale],
                clmns.system_type_id AS [SystemTypeId],
                clmns.user_type_id AS [UserTypeId],
                clmns.xml_collection_id AS [XMLCollectionId],
                CAST(
					CASE WHEN (
						SELECT c.name AS ColumnName
							FROM sys.key_constraints AS k
							JOIN sys.tables AS t ON t.object_id = k.parent_object_id
							JOIN sys.schemas AS s ON s.schema_id = t.schema_id
							JOIN sys.index_columns AS ic ON ic.object_id = t.object_id AND ic.index_id = k.unique_index_id
							JOIN sys.columns AS c ON c.object_id = t.object_id AND c.column_id = ic.column_id
							WHERE t.name=@tablename AND c.name = clmns.name) 
					IS NULL THEN 0 ELSE 1 END AS BIT) AS [IsPrimaryKey]
            FROM
            sys.{1} AS tbl
            INNER JOIN sys.all_columns AS clmns ON clmns.object_id=tbl.object_id
            LEFT OUTER JOIN sys.types AS usrt ON usrt.user_type_id = clmns.user_type_id
            LEFT OUTER JOIN sys.types AS baset ON baset.user_type_id = clmns.system_type_id and 
            baset.user_type_id = baset.system_type_id
            WHERE
            (tbl.name=@tablename and SCHEMA_NAME(tbl.schema_id)=N'{2}')
            ORDER BY
            clmns.column_id ASC";

        private const string _getConstraintsScript = @"SELECT 
                FKTable  = FK.TABLE_NAME, 
                FKColumn = CU.COLUMN_NAME, 
                PKTable  = PK.TABLE_NAME, 
                PKColumn = PT.COLUMN_NAME, 
                ConstraintName = C.CONSTRAINT_NAME 
            FROM 
                INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C 
                INNER JOIN 
                INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK 
                    ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME 
                INNER JOIN 
                INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK 
                    ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME 
                INNER JOIN 
                INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU 
                    ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME 
                INNER JOIN 
                ( 
                    SELECT 
                        i1.TABLE_NAME, i2.COLUMN_NAME 
                    FROM 
                        INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1 
                        INNER JOIN 
                        INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 
                        ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME 
                        WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY' 
                ) PT 
                ON PT.TABLE_NAME = PK.TABLE_NAME 
            -- optional: 
            ORDER BY 
                1,2,3,4";

        private const string _getViews = @"SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='VIEW' AND TABLE_NAME<>'dtproperties' ORDER BY TABLE_SCHEMA, TABLE_NAME";
        private const string _getViewDefiningQuery = @"EXEC sp_helptext '{0}'";
        private const string _getProcedures = "SELECT ROUTINE_NAME, ROUTINE_SCHEMA, ROUTINE_BODY, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE'";
        private const string _getProcedureParameters = @"SELECT PARAMETER_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, PARAMETER_MODE, IS_RESULT FROM information_schema.PARAMETERS WHERE SPECIFIC_NAME = '{0}' AND SPECIFIC_SCHEMA = '{1}' AND SPECIFIC_CATALOG = '{2}'";

        #endregion
        
		public SQLServerDatabaseDriver()
		{
			Datasources = new DatabaseObjectsCollection<SQLServerDatasource>(null);
		}
		
        public override string Name
        {
            get { return "MS SQL Server"; }
        }

        public override string ProviderName
        {
            get { return "System.Data.SqlClient"; }
        }

        public override string ODBCProviderName
        {
            get { return "SQLNCLI10.1"; }
        }

        public override void PopulateDatasources()
        {
            DatabaseObjectsCollection<SQLServerDatasource> datasources = new DatabaseObjectsCollection<SQLServerDatasource>(null);
            
            DataTable dt = SqlDataSourceEnumerator.Instance.GetDataSources();

            foreach (DataRow dr in dt.Rows)
            {
                string serverName = dr["ServerName"].ToString().Trim().ToUpper();
                string instanceName = null;
                Version version = null;

                if (dr["InstanceName"] != null && dr["InstanceName"] != DBNull.Value)
                    instanceName = dr["InstanceName"].ToString().Trim().ToUpper();

                if (dr["Version"] != null && dr["Version"] != DBNull.Value)
                	version = new Version(dr["Version"].ToString().Trim());
                   
                SQLServerDatasource datasource = new SQLServerDatasource(this) { Name = serverName };

                datasource.ProviderManifestToken = GetManifestToken(version);

                if (!String.IsNullOrEmpty(instanceName))
                    datasource.Name += "\\" + instanceName;

                datasources.Add(datasource);
            }

            Datasources = datasources;
        }
		
		string GetManifestToken(Version version)
		{
			string manifestToken;
			if (!IsVersionSupported(version, out manifestToken))
				throw new NotSupportedException(string.Format("Version '{0}' is not supported!", version == null ? "unknown" : version.ToString()));
			return manifestToken;
		}
		
		bool IsVersionSupported(Version version, out string manifestToken)
		{
			manifestToken = "";
			if (version == null)
				return false;
			switch (version.Major) {
				case 8:
					manifestToken = "2000";
					return false;
				case 9:
					manifestToken = "2005";
					return true;
				case 10:
					if (version.Minor == 5) 
						manifestToken = "2008 R2";
					else
						manifestToken = "2008";
					return true;
				case 11:
					manifestToken = "2012";
					return true;
			}
			
			return false;
		}
        
        public override void PopulateDatabases(IDatasource datasource)
        {
            DatabaseObjectsCollection<IDatabase> databases = new DatabaseObjectsCollection<IDatabase>(datasource);
            
            SqlConnection sqlConnection = null;
            sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = datasource.ConnectionString; 
            try
            {
                sqlConnection.Open();
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                { 
                    case 2:
                    case 3:
                    case 53:
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            Datasources.Remove(datasource as SQLServerDatasource);
                        }));
                        break;
                    default:
                        break;
                }

                throw ex;
            }

			Version version = new Version(sqlConnection.ServerVersion);
			datasource.ProviderManifestToken = GetManifestToken(version);

            string sql = string.Empty;

            if (version.Major >= 9)
                sql = "use master; select name from sys.databases order by name";
            else
                sql = "use master; select name from sysdatabases order by name";

            SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
            sqlCommand.CommandTimeout = 20;

            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            
            while (sqlDataReader.Read())
            {
                databases.Add(new Database(datasource) { Name = sqlDataReader["name"].ToString() });
            }

            sqlDataReader.Close();

            datasource.Databases = databases;

            if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
                sqlConnection.Close();
        }

        private void LoadColumns(SqlConnection sqlConnection, ITable table, TableType tableType)
        {
            string tableTypeName = "tables";

            if (tableType == TableType.View)
                tableTypeName = "views";

            using (SqlDataAdapter dataAdapter =
                new SqlDataAdapter(string.Format(_getColumnsScript, table.TableName, tableTypeName, table.SchemaName), sqlConnection))
            {
                DataTable dtColumns = new DataTable("Columns");
                dataAdapter.Fill(dtColumns);

                for (int j = 0; j < dtColumns.Rows.Count; j++)
                {
                    Column column = new Column(table);
                    column.ColumnId = (int)dtColumns.Rows[j]["ColumnId"];
                    column.Name = (string)dtColumns.Rows[j]["Name"];
                    column.DataType = (string)dtColumns.Rows[j]["DataType"];
                    column.SystemType = (string)dtColumns.Rows[j]["SystemType"];
                    column.Length = Convert.ToInt32(dtColumns.Rows[j]["Length"]);

                    if (column.Length == -1)
                    {
                        switch (column.DataType.ToLower())
                        {
                            case "varchar":
                            case "nvarchar":
                                column.DataType += "(max)";
                                break;
                            default:
                                break;
                        }

                        switch (column.SystemType.ToLower())
                        {
                            case "varchar":
                            case "nvarchar":
                                column.SystemType += "(max)";
                                break;
                            default:
                                break;
                        }
                    }

                    column.Precision = Convert.ToInt32(dtColumns.Rows[j]["NumericPrecision"]);
                    column.Scale = Convert.ToInt32(dtColumns.Rows[j]["Scale"]);                    
                    column.IsIdentity = (bool)dtColumns.Rows[j]["IsIdentity"];
                    column.IsNullable = (bool)dtColumns.Rows[j]["IsNullable"];
                    column.IsPrimaryKey = (bool)dtColumns.Rows[j]["IsPrimaryKey"];

                    table.Items.Add(column);
                }
            }
        }

        public override DatabaseObjectsCollection<ITable> LoadTables(IDatabase database)
        {
            DatabaseObjectsCollection<ITable> tables = new DatabaseObjectsCollection<ITable>(database);

            SqlConnection sqlConnection = new SqlConnection(database.ConnectionString);

            using (SqlDataAdapter da = new SqlDataAdapter(_getConstraintsScript, sqlConnection))
            {
                DataTable dtConstraints = new DataTable("Constraints");
                da.Fill(dtConstraints);

                for (int i = 0; i < dtConstraints.Rows.Count; i++)
                {
                    string constraintName = (string)dtConstraints.Rows[i]["ConstraintName"];

                    IConstraint constraint = database.Constraints.FirstOrDefault(c => c.Name == constraintName);

                    if (constraint == null)
                    {
                        constraint = new ICSharpCode.Data.Core.DatabaseObjects.Constraint();
                        constraint.Name = constraintName;
                        constraint.FKTableName = (string)dtConstraints.Rows[i]["FKTable"];
                        constraint.PKTableName = (string)dtConstraints.Rows[i]["PKTable"];

                        database.Constraints.Add(constraint);
                    }

                    constraint.FKColumnNames.Add((string)dtConstraints.Rows[i]["FKColumn"]);
                    constraint.PKColumnNames.Add((string)dtConstraints.Rows[i]["PKColumn"]);                
                }
            }

            using (SqlDataAdapter da = new SqlDataAdapter(_getTables, sqlConnection))
            {
                DataTable dtTables = new DataTable("Tables");
                da.Fill(dtTables);

                for (int i = 0; i < dtTables.Rows.Count; i++)
                {
                    string schemaName = (string)dtTables.Rows[i]["TABLE_SCHEMA"];
                    string tableName = (string)dtTables.Rows[i]["TABLE_NAME"];

                    Table table = new Table() { SchemaName = schemaName, TableName = tableName };
                    LoadColumns(sqlConnection, table, TableType.Table);

                    table.Constraints = database.Constraints.Where(constraint => constraint.FKTableName == tableName).ToDatabaseObjectsCollection(table);
                    tables.Add(table);
                }
            }

            return tables;
        }

        public override DatabaseObjectsCollection<IView> LoadViews(IDatabase database)
        {
            DatabaseObjectsCollection<IView> views = new DatabaseObjectsCollection<IView>(database);

            SqlConnection sqlConnection = new SqlConnection(database.ConnectionString);

            using (SqlDataAdapter da = new SqlDataAdapter(_getViews, sqlConnection))
            {
                DataTable dtViews = new DataTable("Views");
                da.Fill(dtViews);

                for (int i = 0; i < dtViews.Rows.Count; i++)
                {
                    string schemaName = (string)dtViews.Rows[i]["TABLE_SCHEMA"];
                    string viewName = (string)dtViews.Rows[i]["TABLE_NAME"];

                    View view = new View() { SchemaName = schemaName, TableName = viewName, Query = LoadViewQuery(sqlConnection, schemaName, viewName) };            
                    LoadColumns(sqlConnection, view, TableType.View);
                    views.Add(view);
                }
            }

            return views;
        }

        private string LoadViewQuery(SqlConnection sqlConnection, string schemaName, string tableName)
        {
            string definingQuery = string.Empty;
            
            using (SqlDataAdapter dataAdapter =
                new SqlDataAdapter(string.Format(_getViewDefiningQuery, schemaName + "." + tableName), sqlConnection))
            {
                DataTable dtQuery = new DataTable("Text");
                dataAdapter.Fill(dtQuery);

                for (int i = 0; i < dtQuery.Rows.Count; i++)
                {
                    definingQuery += (string)dtQuery.Rows[i]["Text"];
                }
            }

            return definingQuery;
        }

        public override DatabaseObjectsCollection<IProcedure> LoadProcedures(IDatabase database)
        {
            DatabaseObjectsCollection<IProcedure> procedures = new DatabaseObjectsCollection<IProcedure>(database);

            SqlConnection sqlConnection = new SqlConnection(database.ConnectionString);

            using (SqlDataAdapter da = new SqlDataAdapter(_getProcedures, sqlConnection))
            {
                DataTable dtProcedures = new DataTable("Procedures");
                da.Fill(dtProcedures);

                for (int i = 0; i < dtProcedures.Rows.Count; i++)
                {
                    Procedure procedure = new Procedure();
                    procedure.Name = (string)dtProcedures.Rows[i]["ROUTINE_NAME"];
                    procedure.SchemaName = (string)dtProcedures.Rows[i]["ROUTINE_SCHEMA"];
                    if (dtProcedures.Rows[i]["DATA_TYPE"] != DBNull.Value)
                        procedure.DataType = (string)dtProcedures.Rows[i]["DATA_TYPE"];
                    if (dtProcedures.Rows[i]["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value)
                        procedure.Length = Convert.ToInt32(dtProcedures.Rows[i]["CHARACTER_MAXIMUM_LENGTH"]);

                    if (procedure.Length == -1)
                    {
                        switch (procedure.DataType.ToLower())
                        {
                            case "varchar":
                            case "nvarchar":
                                procedure.DataType += "(max)";
                                break;
                            default:
                                break;
                        }
                    }

                    string procedureType = (string)dtProcedures.Rows[i]["ROUTINE_BODY"];
                    if (procedureType == "SQL")
                        procedure.ProcedureType = ProcedureType.SQL;
                    else
                        procedure.ProcedureType = ProcedureType.External;


                    procedure.Items = new DatabaseObjectsCollection<IProcedureParameter>(procedure);

                    DatabaseObjectsCollection<IProcedureParameter> procedureParameters =  new DatabaseObjectsCollection<IProcedureParameter>(procedure);

                    da.SelectCommand = new SqlCommand(string.Format(_getProcedureParameters, procedure.Name, procedure.SchemaName, database.Name), sqlConnection);
                    DataTable dtProcedureParameters = new DataTable("ProcedureParameters");
                    da.Fill(dtProcedureParameters);

                    for (int j = 0; j < dtProcedureParameters.Rows.Count; j++)
                    {
                        if (string.IsNullOrEmpty((string)dtProcedureParameters.Rows[j]["PARAMETER_NAME"]) && 
                            (string)dtProcedureParameters.Rows[j]["IS_RESULT"] == "YES") // = ReturnValue
                            continue;
                        
                        ProcedureParameter procedureParameter = new ProcedureParameter();
                        procedureParameter.Name = (string)dtProcedureParameters.Rows[j]["PARAMETER_NAME"];
                        if (procedureParameter.Name.StartsWith("@"))
                            procedureParameter.Name = procedureParameter.Name.Substring(1);

                        if (dtProcedureParameters.Rows[j]["DATA_TYPE"] != DBNull.Value)
                            procedureParameter.DataType = (string)dtProcedureParameters.Rows[j]["DATA_TYPE"];

                        if (dtProcedureParameters.Rows[j]["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value)
                            procedureParameter.Length = Convert.ToInt32(dtProcedureParameters.Rows[j]["CHARACTER_MAXIMUM_LENGTH"]);

                        string parameterMode = (string)dtProcedureParameters.Rows[j]["PARAMETER_MODE"];
                        if (parameterMode == "IN")
                            procedureParameter.ParameterMode = ParameterMode.In;
                        else if (parameterMode == "OUT")
                            procedureParameter.ParameterMode = ParameterMode.Out;
                        else
                            procedureParameter.ParameterMode = ParameterMode.InOut;
                        
                        procedure.Items.Add(procedureParameter);
                    }

                    procedures.Add(procedure);
                }
            }

            return procedures;
        }
    }
}
