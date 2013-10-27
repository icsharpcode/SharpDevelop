// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.DataManager.Listhandling;
using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Expressions
{
	/// <summary>
	/// Description of ExpressionRunner.
	/// </summary>
	class ExpressionRunner:BaseExporter
	{
		 
//	http://www.killswtch.net/2013/08/01/time-arithmetic-with-irony/
//	http://blog.miraclespain.com/archive/2009/Oct-07.html	
//		
		private readonly ExpressionVisitor visitor;
		private readonly CollectionSource dataSource;
		
		public ExpressionRunner(Collection<ExportPage> pages,ReportSettings reportSettings,CollectionSource dataSource):base(pages)
		{
			this.dataSource = dataSource;
			visitor = new ExpressionVisitor (Pages,reportSettings,dataSource);
			visitor.Evaluator.Globals.Add("DataSource",dataSource);
		}
		
		
		public override void Run()
		{
			Console.WriteLine();
			Console.WriteLine("Start ExpressionVisitor");
			foreach (var page in Pages) {
				var acceptor = page as IAcceptor;
				acceptor.Accept(visitor);
			}
			Console.WriteLine("Finish ExpressionVisitor");
			Console.WriteLine();
		}
	}
}
