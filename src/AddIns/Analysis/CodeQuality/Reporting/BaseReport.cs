// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using ICSharpCode.Reports.Core;

namespace ICSharpCode.CodeQuality.Reporting
{
	/// <summary>
	/// Description of BaseReport.
	/// </summary>
	public class BaseReport
	{
		
		public BaseReport(List <string> fileNames)
		{
			
			if (fileNames.Count > 0)
			{
				this.FileNames = new List<string>();
				this.FileNames.AddRange(fileNames);
			}
		}
		
		protected List<string> FileNames {get;private set;}
		
		public ReportSettings ReportSettings {get;set;}
	}
}
