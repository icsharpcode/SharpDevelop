// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.Expressions;
using ICSharpCode.Reporting.Expressions.Irony;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Exporter.Visitors
{
	/// <summary>
	/// Description of ExpressionVisitor.
	/// </summary>
	class ExpressionVisitor: AbstractVisitor
	{
		private readonly Collection<ExportPage> pages;
		private readonly ReportingLanguageGrammer grammar;
		private readonly ReportingExpressionEvaluator evaluator;
		
		public ExpressionVisitor(Collection<ExportPage> pages,ReportSettings reportSettings):this(reportSettings)
		{
			this.pages = pages;
		}
		
		internal ExpressionVisitor(ReportSettings reportSettings) {
			grammar = new ReportingLanguageGrammer();
			evaluator = new ReportingExpressionEvaluator(grammar);
			evaluator.App.Globals.Add("ReportSettings", reportSettings);
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
			foreach (var element in exportColumn.ExportedItems) {
				var ac = element as IAcceptor;
				ac.Accept(this);
			}
		}
		
		
		public override void Visit(ExportText exportColumn)
		{
			if (ExpressionHelper.CanEvaluate(exportColumn.Text)) {
				try {
					object result = Evaluate(exportColumn);
					exportColumn.Text = result.ToString();
				} catch (Exception e) {
					var s = String.Format("SharpReport.Exprssions -> {0}",e.Message);
					Console.WriteLine(s);
				}
			}
		}

		
		object Evaluate(ExportText exportColumn)
		{
			var str = ExpressionHelper.ExtractExpressionPart(exportColumn.Text);
			var result = evaluator.Evaluate(str);
			return result;
		}
	}
}
