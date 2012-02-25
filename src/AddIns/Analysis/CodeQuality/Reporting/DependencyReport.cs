/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 25.02.2012
 * Time: 21:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using ICSharpCode.CodeQuality.Engine.Dom;
using ICSharpCode.Reports.Core;

namespace ICSharpCode.CodeQuality.Reporting
{
	/// <summary>
	/// Description of Dependencyrepor.
	/// </summary>
	public class DependencyReport:BaseReport
	{
		private const string overviewReport = "DependencyReport.srd";
		public DependencyReport(List<string> fileNames):base(fileNames)
		{
		}
		
		public IReportCreator Run(ReadOnlyCollection<AssemblyNode> list)
		{
			var reportFileName = MakeReportFileName(overviewReport);
			var model = ReportEngine.LoadReportModel(reportFileName);
			ReportSettings = model.ReportSettings;
			var newList = MakeList (list);
			
			IReportCreator creator = ReportEngine.CreatePageBuilder(model,newList,null);
			creator.BuildExportList();
			return creator;
		}
		
		List <DependencyViewModel> MakeList (ReadOnlyCollection<AssemblyNode> list)
		{
			AssemblyNode baseNode = list[0];
			var newList = new List<DependencyViewModel>();
			foreach (var element in list) {
				var i = baseNode.GetUses(element);
				if (i > 0) {
					Console.WriteLine("{0} ref {1} times {2}",baseNode.Name,element.Name,i);
					newList.Add(new DependencyViewModel()
					            {
					            	Node = baseNode,
					            	References = element.Name
					            });
				}
			}
			return newList;
		}
	}
	
	
	internal class DependencyViewModel
	{
		public DependencyViewModel()
		{
			
		}
		
		public AssemblyNode Node {get;set;}
		
			
		public string Name
		{
			get {return Node.Name;}
		}
		
		public string References {get;set;}
	}
		
}
