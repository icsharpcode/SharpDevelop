// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Data;

using ICSharpCode.Reports.Core.DataAccess;
using ICSharpCode.Reports.Core.Interfaces;
using ICSharpCode.Reports.Core.Project.Interfaces;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of DataManagerFacrory.
	/// </summary>
	public static class DataManagerFactory
	{
		
		
		public static IDataManager CreateDataManager (IReportModel reportModel,ReportParameters reportParameters)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			IDataManager dataManager = null;
			ConnectionObject connectionObject = null;
			IDataAccessStrategy accesStrategy = null;
			if (reportParameters != null) {
				connectionObject = reportParameters.ConnectionObject;
			}
			
			if (connectionObject != null) {
				accesStrategy = new SqlDataAccessStrategy(reportModel.ReportSettings,connectionObject);
			} else {
				accesStrategy = new SqlDataAccessStrategy(reportModel.ReportSettings,null);
				
			}
			dataManager = DataManager.CreateInstance(reportModel.ReportSettings,accesStrategy);
			if (dataManager == null) {
				throw new MissingDataManagerException();
			}
			return dataManager;
		}	
		
		
		public static IDataManager CreateDataManager (IReportModel reportModel,DataTable dataTable)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataTable == null) {
				throw new ArgumentNullException("dataTable");
			}
			IDataManager dataManager = DataManager.CreateInstance(dataTable,reportModel.ReportSettings);
			if (dataManager == null) {
				throw new MissingDataManagerException();
			}
			return dataManager;
		}
		
		
		public static IDataManager CreateDataManager (IReportModel reportModel,IList dataTable)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataTable == null) {
				throw new ArgumentNullException("dataTable");
			}
			IDataManager dataManager = DataManager.CreateInstance(dataTable,reportModel.ReportSettings);
			if (dataManager == null) {
				throw new MissingDataManagerException();
			}
			return dataManager;
		}
	}
}
