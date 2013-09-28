// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Exporter.Visitors;
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
		
		 public ExpressionRunner(Collection<ExportPage> pages):base(pages)
		{
			visitor = new ExpressionVisitor (Pages);
			
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
