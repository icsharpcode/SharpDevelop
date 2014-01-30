// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
			
//			if (connectionObject != null) {
//				accesStrategy = new SqlDataAccessStrategy(reportModel.ReportSettings,connectionObject);
//			} else {
//				accesStrategy = new SqlDataAccessStrategy(reportModel.ReportSettings,null);
//				
//			}
			accesStrategy = new SqlDataAccessStrategy(reportModel.ReportSettings);
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
