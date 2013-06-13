// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

using ICSharpCode.Reports.Core.Project.Interfaces;

namespace ICSharpCode.Reports.Core.DataAccess
{
	/// <summary>
	/// Description of DataAccess.
	/// </summary>
	/// 
	
	public class SqlDataAccessStrategy:IDataAccessStrategy
	{
		private ReportSettings reportSettings;
		
		public SqlDataAccessStrategy(ReportSettings reportSettings)
		{
			if (reportSettings == null) {
				throw new ArgumentNullException("reportSettings");
			}
			this.reportSettings = reportSettings;
		}
		
		
		public DataSet ReadData ()
		{
			using (SqlConnection connection = new SqlConnection(this.reportSettings.ConnectionString)){
				connection.Open();
				
				using (SqlCommand command = connection.CreateCommand()) {
					command.CommandText = reportSettings.CommandText;
					command.CommandType = reportSettings.CommandType;
					BuildQueryParameters(command,reportSettings.SqlParameters);
					using (SqlDataAdapter adapter = new SqlDataAdapter(command)){
						DataSet ds = new DataSet();
						ds.Locale = CultureInfo.CurrentCulture;
						adapter.Fill (ds);
						return ds;
					}
				}
			}
		}
		
		
		private static void BuildQueryParameters (IDbCommand cmd,SqlParameterCollection parameterCollection){
		                                         
			if (parameterCollection != null && parameterCollection.Count > 0) {
				foreach (SqlParameter par in  parameterCollection) {
					var cmdPar = cmd.CreateParameter();
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
