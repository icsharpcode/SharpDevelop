// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using Irony.Interpreter.Evaluator;

namespace ICSharpCode.Reporting.Exporter.Visitors
{
	/// <summary>
	/// Description of ExpressionVisitor.
	/// </summary>
	class ExpressionVisitor: AbstractVisitor
	{
		private readonly Collection<ExportPage> pages;
		private readonly ExpressionEvaluatorGrammar grammar;
		private readonly ExpressionEvaluator evaluator;
		
		public ExpressionVisitor(Collection<ExportPage> pages)
		{
			this.pages = pages;
			grammar = new ExpressionEvaluatorGrammar();
			evaluator = new ExpressionEvaluator(grammar);
		}
		
		public override void Visit(ICSharpCode.Reporting.PageBuilder.ExportColumns.ExportColumn exportColumn)
		{
			Console.WriteLine("Visit Page ");
			
		}
		
		public override void Visit(ICSharpCode.Reporting.PageBuilder.ExportColumns.ExportText exportColumn)
		{
			Console.WriteLine("\t\t<ExportText> {0} - {1}", exportColumn.Text,exportColumn.Location);
			var result = evaluator.Evaluate("2 + 3");
			Console.WriteLine("ExpressionVisitor <{0}> - {1}",exportColumn.Name,result);
			
		}
		
		public override void Visit(ICSharpCode.Reporting.PageBuilder.ExportColumns.ExportContainer exportColumn)
		{
			Console.WriteLine("\t{0} - {1}  Items {2}",
			                  exportColumn.Name,exportColumn.Location,exportColumn.BackColor);
			foreach (var element in exportColumn.ExportedItems) {
				var ac = element as IAcceptor;
				ac.Accept(this);
			}
			var result = evaluator.Evaluate("2 * 10");
			Console.WriteLine("ExpressionVisitor <{0}> - {1}",exportColumn.Name,result);
			
			
		}
		
		public override void Visit(ICSharpCode.Reporting.PageBuilder.ExportColumns.ExportPage page)
		{
			foreach (var element in page.ExportedItems) {
				var ac = element as IAcceptor;
				ac.Accept(this);
			}
			var result = evaluator.Evaluate("5 * 10");
//			Console.WriteLine("ExpressionVisitor <{0} - {1}>",exportColumn.Name,result);
			Console.WriteLine("ExpressionVisitor page <{0}> {1}",page.PageInfo.PageNumber,result);
		}
	}
}
