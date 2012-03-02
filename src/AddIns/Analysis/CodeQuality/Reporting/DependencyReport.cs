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
using System.Reflection;

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
			System.Reflection.Assembly asm = Assembly.GetExecutingAssembly();
			System.IO.Stream stream = asm.GetManifestResourceStream("ICSharpCode.CodeQuality.Reporting.DependencyReport.srd");
			var model = ReportEngine.LoadReportModel(stream);
			ReportSettings = model.ReportSettings;
			var newList = MakeList (list);
			
			IReportCreator creator = ReportEngine.CreatePageBuilder(model,newList,null);
			creator.BuildExportList();
			return creator;
		}
		
		
		private List <DependencyViewModel> MakeList (ReadOnlyCollection<AssemblyNode> list)
		{
			var newList = new List<DependencyViewModel>();
			foreach (var baseNode in list) {
				foreach (var element in list) {
					if (baseNode.Name != element.Name) {
						
					
					var referenceCount = baseNode.GetUses(element);
					if (referenceCount > 0) {
						newList.Add(new DependencyViewModel()
						            {
						            	Node = baseNode,
						            	References = element.Name,
						            	ReferenceCount = referenceCount
						            });
					}
					}
				}
			}
			return newList;
		}
	}
	
	
	internal class DependencyViewModel:ReportViewModel
	{
		public DependencyViewModel()
		{
			
		}
		
		public string References {get;set;}
		
		public int ReferenceCount {get;set;}
	}	
}
