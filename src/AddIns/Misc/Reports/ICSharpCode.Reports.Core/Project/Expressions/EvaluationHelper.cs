/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 30.01.2011
 * Time: 19:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
