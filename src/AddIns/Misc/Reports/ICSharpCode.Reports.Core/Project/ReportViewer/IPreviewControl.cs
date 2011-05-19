// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Windows.Forms;

using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Exporter.ExportRenderer;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.ReportViewer
{
	public interface IPreviewControl
	{
		void RunReport(string fileName, ReportParameters parameters);
		void RunReport(ReportModel reportModel, ReportParameters parameters);
		void RunReport(ReportModel reportModel, DataTable dataTable, ReportParameters parameters);
		void RunReport(ReportModel reportModel, IList dataSource, ReportParameters parameters);
		void RunReport(ReportModel reportModel, IDataManager dataManager);
		PagesCollection Pages { get; }
		IReportViewerMessages Messages { get; set; }
	}
}
