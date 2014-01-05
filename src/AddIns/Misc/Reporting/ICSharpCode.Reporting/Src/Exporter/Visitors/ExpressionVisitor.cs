// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using ICSharpCode.Reporting.DataManager.Listhandling;
using ICSharpCode.Reporting.Expressions;
using ICSharpCode.Reporting.Expressions.Irony;
using ICSharpCode.Reporting.Expressions.Irony.Ast;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using System.Collections.Generic;
namespace ICSharpCode.Reporting.Exporter.Visitors
{
	/// <summary>
	/// Description of ExpressionVisitor.
	/// </summary>
	class ExpressionVisitor: AbstractVisitor
	{
		readonly ReportingLanguageGrammer grammar;
		readonly ReportingExpressionEvaluator evaluator;
		
		
		public ExpressionVisitor(ReportSettings reportSettings,CollectionDataSource dataSource) {
			grammar = new ReportingLanguageGrammer();
			evaluator = new ReportingExpressionEvaluator(grammar);
			evaluator.AddReportSettings(reportSettings);
			if (dataSource != null) {
				evaluator.AddDataSource(dataSource);
			}
		}
		
		
		public void SetSourceList (IEnumerable<object> list) {
			
		}
		
		
		public override void Visit(ExportPage page)
		{
			evaluator.AddPageInfo(page.PageInfo);
			base.Visit(page);
		}
		
		
		public override void Visit(ExportContainer exportContainer)
		{
			evaluator.AddCurrentContainer(exportContainer);
			base.Visit(exportContainer);
		}
		
		
		public override void Visit(ExportText exportColumn)
		{
			if (ExpressionHelper.CanEvaluate(exportColumn.Text)) {
				try {
					object result = Evaluate(exportColumn);
					exportColumn.Text = result.ToString();
				} catch (Exception e) {
					var s = String.Format("SharpReport.Expressions -> {0} for {1}",e.Message,exportColumn.Text);
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
		
		public ReportingExpressionEvaluator Evaluator {
			get { return evaluator; }
		}
		
	}
}
