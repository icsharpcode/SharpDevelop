// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

using ICSharpCode.NRefactory.Utils;
using ICSharpCode.Reporting;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Items;
using ICSharpCode.CodeQuality.Engine.Dom;

namespace ICSharpCode.CodeQuality.Reporting
{
	/// <summary>
	/// Description of Overview.
	/// </summary>
	public class OverviewReport:BaseReport
	{
		private const string overviewReport = "OverviewReport.srd";
		ReadOnlyCollection<AssemblyNode> list;
		
		public  OverviewReport(List<string> fileNames):base(fileNames)
		{
		}
		
		public IReportCreator Run(ReadOnlyCollection<AssemblyNode> list)
		{
			this.list = list;
			var asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream("ICSharpCode.CodeQuality.Reporting.Overviewreport.srd");
			
			var viewModelList = CreateViewModel(list);
			
			var reportingFactory = new ReportingFactory();
			var reportCreator = reportingFactory.ReportCreator (stream,viewModelList);
			ReportSettings = reportingFactory.ReportModel.ReportSettings;
//			var reportParameters = new ParameterCollection();
//			reportParameters.Add(new BasicParameter ("param1",base.FileNames[0]));
//			reportParameters.Add(new BasicParameter ("param2",list.Count.ToString()));
//			
//			ReportSettings.ParameterCollection.AddRange(reportParameters);
			reportCreator.SectionRendering += HandleSectionEvents;
			reportCreator.BuildExportList();
			return reportCreator;
		}

		
		static IEnumerable<OverviewViewModel> CreateViewModel(ReadOnlyCollection<AssemblyNode> list)
		{
			var newList = from c in list
			              select new OverviewViewModel {
				Node = c
			};
			return newList;
		}
		
		
		void HandleSectionEvents(object sender, SectionEventArgs e)
		{
			var sectionName = e.Section.Name;
			if (sectionName == ReportSectionNames.ReportHeader) {

				var param1 = (BaseTextItem)e.Section.Items.FirstOrDefault(n => n.Name == "Param1");
//				FileInfo fi =new FileInfo(FileNames[0]);
//			var s = fi..Directory + fi.Name;
				param1.Text = FileNames[0];
				var param2 = (BaseTextItem)e.Section.Items.FirstOrDefault(n => n.Name == "Param2");
				param2.Text = list.Count.ToString();
			}
			
			else if (sectionName == ReportSectionNames.ReportPageHeader) {
				Console.WriteLine("PushPrinting :" +ReportSectionNames .ReportPageHeader);
			}
			
			else if (sectionName == ReportSectionNames.ReportDetail){
//				Console.WriteLine("PushPrinting :" + ReportSectionNames.ReportDetail);
			}
			
			else if (sectionName == ReportSectionNames.ReportPageFooter){
//				Console.WriteLine("PushPrinting :" + ReportSectionNames.ReportPageFooter);
			}
			
			else if (sectionName == ReportSectionNames.ReportFooter){
//				Console.WriteLine("PushPrinting :" + ReportSectionNames.ReportFooter);
			}
		}
	}
	
	
	class OverviewViewModel:ReportViewModel
	{
		public OverviewViewModel ()
		{
		}
		
		
		public int 	ChildCount
		{
			get {return Node.Children.Count;}	
		}
		
		
		public int MethodCount
		{
			get
			{
				return TreeTraversal.PreOrder((NodeBase)Node, n => n.Children).OfType<MethodNode>().Count();
			}
		}
		
		
		public int PropertyCount
		{
			get
			{
				return TreeTraversal.PreOrder((NodeBase)Node, n => n.Children).OfType<PropertyNode>().Count();
			}
		}
		
		
		public int EventCount
		{
			get
			{
				return TreeTraversal.PreOrder((NodeBase)Node, n => n.Children).OfType<EventNode>().Count();
			}
		}
		
		
		public int TypesCount
		{
			get
			{
				return TreeTraversal.PreOrder((NodeBase)Node, n => n.Children).OfType<TypeNode>().Count();
			}
		}
	}
}
