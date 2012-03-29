/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 25.02.2012
 * Time: 21:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
