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
using System.Linq;
using System.Reflection;
using ICSharpCode.CodeQuality.Engine.Dom;
using ICSharpCode.CodeQuality.Reporting;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.Reports.Core;

namespace ICSharpCode.CodeQuality.Reporting
{
	/// <summary>
	/// Description of Overview.
	/// </summary>
	public class OverviewReport:BaseReport
	{
		private const string overviewReport = "ReportingOverview.srd";
		
		public  OverviewReport(List<string> fileNames):base(fileNames)
		{
		}
		
		public IReportCreator Run(ReadOnlyCollection<AssemblyNode> list)
		{
			
			var reportFileName = MakeReportFileName(overviewReport);
			var model = ReportEngine.LoadReportModel(reportFileName);
			ReportSettings = model.ReportSettings;
			
			var	 r =  from c in list
				select new OverviewViewModel { Node = c};
			
			var p = new ReportParameters();
			p.Parameters.Add(new BasicParameter ("param1",base.FileNames[0]));
			p.Parameters.Add(new BasicParameter ("param2",list.Count.ToString()));
			
			IReportCreator creator = ReportEngine.CreatePageBuilder(model,r.ToList(),p);
			creator.BuildExportList();
			return creator;
		}	
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
		
		
		public int MethodCount
		{
			get {
				return TreeTraversal.PreOrder((NodeBase)Node, n => n.Children).OfType<MethodNode>().Count();
			}
		}
		
		
		public int PropertyCount
		{
			get {
				return TreeTraversal.PreOrder((NodeBase)Node, n => n.Children).OfType<PropertyNode>().Count();
			}
		}
		
		
		public int EventCount
		{
			get {
				return TreeTraversal.PreOrder((NodeBase)Node, n => n.Children).OfType<EventNode>().Count();
			}
		}
		
		
		public int TypesCount
		{
			get {
				return TreeTraversal.PreOrder((NodeBase)Node, n => n.Children).OfType<TypeNode>().Count();
			}
		}
	}
}
