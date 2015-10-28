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

using System.Collections.ObjectModel;
using ICSharpCode.Reporting.DataManager.Listhandling;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Expressions
{
	/// <summary>
	/// Description of ExpressionRunner.
	/// </summary>
	class ExpressionRunner
	{
		 
//	http://www.killswtch.net/2013/08/01/time-arithmetic-with-irony/
//	http://blog.miraclespain.com/archive/2009/Oct-07.html	
//		
		readonly Collection<ExportPage> pages;
		readonly CollectionDataSource dataSource;
		
		public ExpressionRunner(Collection<ExportPage> pages,IReportSettings reportSettings,CollectionDataSource dataSource){
			this.pages = pages;
			this.dataSource = dataSource;
			Visitor = new ExpressionVisitor(reportSettings);
		}
		
		
		public  void Run(){
			if (dataSource != null) {
				if (dataSource.SortedList != null) {
					Visitor.SetCurrentDataSource(dataSource.SortedList);
				}
				if (dataSource.GroupedList != null) {
					Visitor.SetCurrentDataSource(dataSource.GroupedList);
				}
			}
			Visitor.Run(pages);
		}
		
		public ExpressionVisitor Visitor {get; private set;}
	}
}
