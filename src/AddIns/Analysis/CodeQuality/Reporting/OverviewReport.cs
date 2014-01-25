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

using ICSharpCode.CodeQuality.Engine.Dom;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.Reports.Core;

namespace ICSharpCode.CodeQuality.Reporting
{
	/// <summary>
	/// Description of Overview.
	/// </summary>
	public class OverviewReport:BaseReport
	{
		private const string overviewReport = "OverviewReport.srd";
		
		public  OverviewReport(List<string> fileNames):base(fileNames)
		{
		}
		
		public IReportCreator Run(ReadOnlyCollection<AssemblyNode> list)
		{
			System.Reflection.Assembly asm = Assembly.GetExecutingAssembly();
			System.IO.Stream stream = asm.GetManifestResourceStream("ICSharpCode.CodeQuality.Reporting.Overviewreport.srd");
			var model = ReportEngine.LoadReportModel(stream);
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
	
	
	internal class OverviewViewModel:ReportViewModel
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
