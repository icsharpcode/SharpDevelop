/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 13.02.2012
 * Time: 19:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Linq;

using ICSharpCode.CodeQuality.Engine.Dom;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Exporter.ExportRenderer;
using ICSharpCode.Reports.Core.WpfReportViewer;
using Microsoft.Win32;

namespace ICSharpCode.CodeQuality.Reporting
{
	/// <summary>
	/// Description of Overview.
	/// </summary>
	public class OverviewReport
	{
		List<string> fileNames;
		public  OverviewReport(List<string> fileNames)
		{
			if (fileNames.Count > 0)
			{
				this.fileNames = new List<string>();
				this.fileNames.AddRange(fileNames);
			}
		}
		
		public IReportCreator Run(ReadOnlyCollection<AssemblyNode> list)
		{
			
			var	 r =  from c in list
				select new OverviewViewModel { Node = c};
			

			Uri uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
			var fullname = uri.LocalPath;
			var reportFileName = Path.GetDirectoryName(fullname) + Path.DirectorySeparatorChar + "Reporting" + Path.DirectorySeparatorChar + "Report1.srd";
			
			var model = ReportEngine.LoadReportModel(reportFileName);
			var p = new ReportParameters();
			p.Parameters.Add(new BasicParameter ("param1",fileNames[0]));
			
			ReportSettings = model.ReportSettings;

			
			IReportCreator creator = ReportEngine.CreatePageBuilder(model,r.ToList(),p);
			creator.BuildExportList();
			return creator;
		}
		
		public ReportSettings ReportSettings {get;private set;}
			
	}
	
	
	internal class OverviewViewModel 
	{
		public OverviewViewModel ()
		{
		}
		
		public AssemblyNode Node {get;set;}
		
		public string Name
		{
			get {return Node.Name;}
		}
		
		public int 	ChildCount
		{
			get {
				
				return Node.Children.Count;}
		}
		
		
	}
}
