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
		private const string reportDir = "Reporting";
		
		public BaseReport(List <string> fileNames)
		{
			
			if (fileNames.Count > 0)
			{
				this.FileNames = new List<string>();
				this.FileNames.AddRange(fileNames);
			}
		}
		
		protected string MakeReportFileName (string reportName)
		{
			Uri uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
			var fullname = uri.LocalPath;
			return  Path.GetDirectoryName(fullname) + Path.DirectorySeparatorChar + reportDir + Path.DirectorySeparatorChar + reportName;
		} 
		
		protected List<string> FileNames {get;private set;}
		
		public ReportSettings ReportSettings {get;set;}
	}
}
