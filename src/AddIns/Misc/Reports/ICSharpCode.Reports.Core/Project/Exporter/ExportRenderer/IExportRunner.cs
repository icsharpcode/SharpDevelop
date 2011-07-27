// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Data;

namespace ICSharpCode.Reports.Core.Exporter.ExportRenderer
{
	public interface IExportRunner
	{
		void RunReport(string fileName, ReportParameters parameters);
		void RunReport(ReportModel reportModel, ReportParameters parameters);
		void RunReport(ReportModel reportModel, DataTable dataTable, ReportParameters parameters);
		void RunReport(ReportModel reportModel, IList dataSource, ReportParameters parameters);
		void RunReport(ReportModel reportModel, IDataManager dataManager);
		PagesCollection Pages { get; }
		event EventHandler<PageCreatedEventArgs> PageCreated;
	}
}
