// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.Expressions;
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
		
		internal ExpressionVisitor() {
			grammar = new ExpressionEvaluatorGrammar();
			evaluator = new ExpressionEvaluator(grammar);
		}
		
		public override void Visit(ExportPage page)
		{
			var result = evaluator.Evaluate("5 * 10");
			Console.WriteLine("ExpressionVisitor page <{0}> {1}",page.PageInfo.PageNumber,result);
			foreach (var element in page.ExportedItems) {
				var ac = element as IAcceptor;
				ac.Accept(this);
			}
		}
	
		
		public override void Visit(ExportContainer exportColumn)
		{
			var result = evaluator.Evaluate("2 * 10");
			Console.WriteLine("\tExpressionVisitor <{0}> - {1}",exportColumn.Name,result);
//			Console.WriteLine("\t{0} - {1}  Items {2}",
//			                  exportColumn.Name,exportColumn.Location,exportColumn.BackColor);
			foreach (var element in exportColumn.ExportedItems) {
				var ac = element as IAcceptor;
				ac.Accept(this);
			}
		}
		
		
		public override void Visit(ExportText exportColumn)
		{
			if (exportColumn.Text.StartsWith("=")) {
				try {
					var str = ExpressionHelper.ExtractExpressionPart(exportColumn.Text);
				var result = evaluator.Evaluate(str);
				exportColumn.Text = result.ToString();
				} catch (Exception) {
					
					throw;
				}
			}
		}
	}
}
