// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.DataManager.Listhandling;
using ICSharpCode.Reporting.Exporter.Visitors;
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
		readonly ReportSettings reportSettings;
		readonly CollectionDataSource dataSource;
		
		public ExpressionRunner(Collection<ExportPage> pages,ReportSettings reportSettings,CollectionDataSource dataSource)
		{
			this.pages = pages;
			this.dataSource = dataSource;
			this.reportSettings = reportSettings;
			Visitor = new ExpressionVisitor(reportSettings);
		}
		
		
		public  void Run()
		{
			Visitor = new ExpressionVisitor (reportSettings);
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
