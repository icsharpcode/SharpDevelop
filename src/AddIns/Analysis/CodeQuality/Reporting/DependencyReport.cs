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
using System.ComponentModel;
using System.Reflection;

using ICSharpCode.Reporting;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Items;
using ICSharpCode.CodeQuality.Engine.Dom;

namespace ICSharpCode.CodeQuality.Reporting
{
	/// <summary>
	/// Description of Dependencyrepor.
	/// </summary>
	public class DependencyReport:BaseReport
	{
		const string overviewReport = "DependencyReport.srd";
		
		public DependencyReport(List<string> fileNames):base(fileNames)
		{
		}
		
		public IReportCreator Run(ReadOnlyCollection<AssemblyNode> list)
		{
			var newList = MakeList (list);
			var asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream("ICSharpCode.CodeQuality.Reporting.DependencyReport.srd");
			var reportingFactory = new ReportingFactory();
			var reportCreator = reportingFactory.ReportCreator (stream,newList);
			ReportSettings = reportingFactory.ReportModel.ReportSettings;
			var groupColumn = (GroupColumn)ReportSettings.GroupColumnsCollection[0];
			groupColumn.GroupSortColumn = new SortColumn() {
				ColumnName = "ReferenceCount",
				SortDirection = ListSortDirection.Ascending
			};
			reportCreator.BuildExportList();
			return reportCreator;
		}

		
		List <DependencyViewModel> MakeList (ReadOnlyCollection<AssemblyNode> list)
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
	
	
	class DependencyViewModel:ReportViewModel
	{
		public DependencyViewModel()
		{
			
		}
		
		public string References {get;set;}
		
		public int ReferenceCount {get;set;}
	}	
}
