// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Core.Interfaces
{
	public interface ISinglePage
	{
		void CalculatePageBounds(IReportModel reportModel);
		
		
		SectionBounds SectionBounds {get;set;}
		
		int StartRow {get;set;}
		int EndRow {get;set;}
		int PageNumber {get;set;}
		int TotalPages {get;set;}
		string ReportName {get;set;}
		string ReportFileName {get;set;}
		string ReportFolder {get;}
		DateTime ExecutionTime {get;set;}
		System.Collections.Hashtable ParameterHash {get;set;}
		IDataNavigator IDataNavigator {get;set;}
	}
}
