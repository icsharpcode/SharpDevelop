// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Reports.Core.Globals
{
	/// <summary>
	/// Description of ReportSectionNames.
	/// </summary>
	public static class ReportSectionNames
	{
		public static string ReportHeader
		{
			get{ return GlobalEnums.ReportSection.ReportHeader.ToString();}
		}
		
		public static string ReportPageHeader
		{
			get{ return GlobalEnums.ReportSection.ReportPageHeader.ToString();}
		}
		
		
		public static string ReportDetail
		{
			get { return GlobalEnums.ReportSection.ReportDetail.ToString();}
		}
		
		public static string ReportPageFooter
		{
			get { return GlobalEnums.ReportSection.ReportPageFooter.ToString();}
		}
		
		public static string ReportFooter
		{
			get{ return GlobalEnums.ReportSection.ReportFooter.ToString();}
		}
	}
}
