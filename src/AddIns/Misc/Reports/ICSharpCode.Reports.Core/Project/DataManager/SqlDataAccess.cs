// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Data;
using System.Globalization;

using ICSharpCode.Reports.Core.Project.Interfaces;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of DataAccess.
	/// </summary>
	/// 
	
	internal class SqlDataAccessStrategy:IDataAccessStrategy
	{
		private ConnectionObject  connectionObject;
		private ReportSettings reportSettings;
		
		public SqlDataAccessStrategy(ReportSettings reportSettings,ConnectionObject connectionObject)
		{
			if (reportSettings == null) {
				throw new ArgumentNullException("reportSettings");
			}
			this.reportSettings = reportSettings;
			if (connectionObject == null) {
				this.connectionObject = ConnectionObjectFactory.BuildConnectionObject(reportSettings.ConnectionString);
			} else {
				this.connectionObject = connectionObject;
			}
		}
		
		
		public bool OpenConnection ()
		{
			CheckConnection();
			return true;
		}
		
		
		
		private void CheckConnection()
		{
			if (this.connectionObject.Connection.State == ConnectionState.Open) {
				this.connectionObject.Connection.Close();
			}
			this.connectionObject.Connection.Open();
		}
		
		
		
		public DataSet ReadData()
		{
			try {
				if (this.connectionObject.Connection.State == ConnectionState.Closed) {
					this.connectionObject.Connection.Open();
				}
				
				IDbCommand command = this.connectionObject.Connection.CreateCommand();
				
				if (String.IsNullOrEmpty(this.connectionObject.QueryString)) {
					command.CommandText = reportSettings.CommandText;
				} else {
					command.CommandText = this.connectionObject.QueryString;
				}
				
				command.CommandType = reportSettings.CommandType;
				// We have to check if there are parameters for this Query, if so
				// add them to the command
				
				BuildQueryParameters(command,reportSettings.ParameterCollection);
				
				IDbDataAdapter adapter = connectionObject.CreateDataAdapter(command);
				DataSet ds = new DataSet();
				ds.Locale = CultureInfo.CurrentCulture;
				adapter.Fill (ds);
				return ds;
				
			} finally {
				if (this.connectionObject.Connection.State == ConnectionState.Open) {
					this.connectionObject.Connection.Close();
				}
			}
		}
		
		
		public static void BuildQueryParameters (IDbCommand cmd,
		                                         ParameterCollection parameterCollection)
		{
			if (parameterCollection != null && parameterCollection.Count > 0) {
				
				IDbDataParameter cmdPar = null;
				System.Collections.Generic.List<SqlParameter> sq = parameterCollection.ExtractSqlParameters();
			
				foreach (SqlParameter par in  sq) {
					cmdPar = cmd.CreateParameter();
					cmdPar.ParameterName = par.ParameterName;
					
					if (par.DataType != System.Data.DbType.Binary) {
						cmdPar.DbType = par.DataType;
						cmdPar.Value = par.ParameterValue;

					} else {
						cmdPar.DbType = System.Data.DbType.Binary;
					}
					cmdPar.Direction = par.ParameterDirection;
					cmd.Parameters.Add(cmdPar);
				}
			}
		}
	}
}
