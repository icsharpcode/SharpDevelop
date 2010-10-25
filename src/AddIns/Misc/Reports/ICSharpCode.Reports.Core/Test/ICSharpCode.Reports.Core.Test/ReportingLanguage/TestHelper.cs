// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Reports.Core.BaseClasses;

namespace ICSharpCode.Reports.Core.Test.ReportingLanguage
{
	/// <summary>
	/// Description of TestHelper.
	/// </summary>
	public class TestHelper
	{
		
		public static SinglePage CreateSinglePage ()
		{
			SectionBounds sb = new SectionBounds(new ReportSettings(),false);
			SinglePage p = new SinglePage(sb,15);
		
			p.TotalPages = 25;
			p.ReportName = "SharpTestReport.srd";
			p.ReportFileName =@"c:\testreports\SharpTestReport.srd";
			p.ExecutionTime = new DateTime(2009,12,24,23,59,59);
			return p;
		}
	}
}
