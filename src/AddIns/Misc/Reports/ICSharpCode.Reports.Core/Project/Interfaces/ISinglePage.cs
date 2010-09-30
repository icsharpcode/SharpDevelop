// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.BaseClasses;

namespace ICSharpCode.Reports.Core.Interfaces
{
	
	public interface IPageInfo
	{
		int PageNumber {get;set;}
		int TotalPages {get;set;}
		string ReportName {get;set;}
		string ReportFileName {get;set;}
		string ReportFolder {get;}
		DateTime ExecutionTime {get;set;}
		System.Collections.Hashtable ParameterHash {get;set;}
		IDataNavigator IDataNavigator {get;set;}
	}
	
	
	public interface ISinglePage:IPageInfo
	{
		void CalculatePageBounds(IReportModel reportModel);
		SectionBounds SectionBounds {get;set;}
	}
}
