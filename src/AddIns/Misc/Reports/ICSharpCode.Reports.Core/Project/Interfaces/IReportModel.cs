// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Reports.Core.Globals;

namespace ICSharpCode.Reports.Core.Interfaces
{
	/// <summary>
	/// Description of IReportModel.
	/// </summary>
	public interface IReportModel
	{
		BaseSection ReportHeader {get;}
		BaseSection PageHeader {get;}
		BaseSection DetailSection {get;}
		BaseSection PageFooter {get;}
		BaseSection ReportFooter {get;}
		ReportSettings ReportSettings {get;set;}
		GlobalEnums.PushPullModel DataModel {get;}
		ReportSectionCollection SectionCollection {get;}
	}
}
