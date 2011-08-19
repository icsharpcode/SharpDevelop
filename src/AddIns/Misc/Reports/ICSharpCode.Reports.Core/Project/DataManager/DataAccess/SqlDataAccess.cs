// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Data;
using System.Globalization;

using ICSharpCode.Reports.Core.Factories;
using ICSharpCode.Reports.Core.Project.Interfaces;

namespace ICSharpCode.Reports.Core.DataAccess
{
	/// <summary>
	/// Description of DataAccess.
	/// </summary>
	/// 
	
	public class SqlDataAccessStrategy:IDataAccessStrategy
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
				this.connectionObject = ConnectionObjectFactory.BuildConnectionObject(reportSettings);
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
				
				BuildQueryParameters(command,reportSettings.SqlParameters);
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
		
		
		private static void BuildQueryParameters (IDbCommand cmd,
		                                         SqlParameterCollection parameterCollection)
		{
			if (parameterCollection != null && parameterCollection.Count > 0) {
				
				IDbDataParameter cmdPar = null;

				foreach (SqlParameter par in  parameterCollection) {
					cmdPar = cmd.CreateParameter();
					cmdPar.ParameterName = par.ParameterName;
					Console.WriteLine("");
					Console.WriteLine("BuildQueryParameters {0} - {1}",par.ParameterName,par.ParameterValue);
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
