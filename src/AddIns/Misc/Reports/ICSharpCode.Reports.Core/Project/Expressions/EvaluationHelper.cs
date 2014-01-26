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
using System.Globalization;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.Interfaces;
using SimpleExpressionEvaluator;

namespace ICSharpCode.Reports.Expressions.ReportingLanguage
{
	/// <summary>
	/// Description of EvaluationHelper.
	/// </summary>
	internal class EvaluationHelper
	{
		
		public static IExpressionEvaluatorFacade  SetupEvaluator ()
		{
			return new ExpressionEvaluatorFacade(null);
		}
		
		
		public static IExpressionEvaluatorFacade  CreateEvaluator (ISinglePage singlePage,IDataNavigator dataNavigator)
		{
			if (singlePage == null) {
				
				throw new ArgumentNullException("singlePage");
			}
			singlePage.IDataNavigator = dataNavigator;
			IExpressionEvaluatorFacade evaluatorFacade = new ExpressionEvaluatorFacade(singlePage);
			return evaluatorFacade;
		}
		
		
		public static void EvaluateReportItems (IExpressionEvaluatorFacade  evaluator,ReportItemCollection items)
		{
			foreach(BaseReportItem column in items)
			{
				var container = column as ISimpleContainer ;
				if (container != null) {
					EvaluateReportItems(evaluator,container.Items);
				}
				
				IReportExpression expressionItem = column as IReportExpression;
				if (expressionItem != null) {
					evaluator.Evaluate(expressionItem);
				}
			}
		}

		
		public static void EvaluateRow(IExpressionEvaluatorFacade evaluator,ExporterCollection row)
		{
			foreach (BaseExportColumn column in row) {
				var container = column as IExportContainer;
				if (container != null) {
					EvaluateRow(evaluator,container.Items);
				}
				
				IReportExpression expressionItem = column as IReportExpression;
				if (expressionItem != null) {
					evaluator.Evaluate(expressionItem);
				}
			}
		}
		
		
		public static bool CanEvaluate (string expression)
		{
			if ((!String.IsNullOrEmpty(expression)) && (expression.StartsWith("=",StringComparison.InvariantCultureIgnoreCase))) {
				return true;
			}
			return false;
		}
		
		
		public static string ExtractExpressionPart (string src)
		{
			char v = Convert.ToChar("=",CultureInfo.CurrentCulture );
			return StringHelpers.RightOf(src,v).Trim();
		}
		
		public static string ExtractResultPart (string src)
		{
			char v = Convert.ToChar("=",CultureInfo.CurrentCulture );
			return StringHelpers.LeftOf(src,v).Trim();
		}
	}
}
