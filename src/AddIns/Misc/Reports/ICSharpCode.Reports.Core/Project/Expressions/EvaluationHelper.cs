/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 30.01.2011
 * Time: 19:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Interfaces;
using SimpleExpressionEvaluator;

namespace ICSharpCode.Reports.Expressions.ReportingLanguage
{
	/// <summary>
	/// Description of EvaluationHelper.
	/// </summary>
	public class EvaluationHelper
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
			/*
			if (dataNavigator == null) {
				throw new ArgumentNullException("dataNavigator");
			}
			 * */
			singlePage.IDataNavigator = dataNavigator;
			IExpressionEvaluatorFacade evaluatorFacade = new ExpressionEvaluatorFacade(singlePage);
			return evaluatorFacade;
		}
		
		
		public static void EvaluateReportItems (IExpressionEvaluatorFacade  evaluator,ReportItemCollection items)
		{
			foreach(BaseReportItem column in items) {
				var container = column as ISimpleContainer ;
				if (container != null) {
					EvaluateReportItems(evaluator,container.Items);
				}
				
				IReportExpression expressionItem = column as IReportExpression;
				if (expressionItem != null) {
					EvaluateItem(evaluator,expressionItem);
				}
			}
		}

		public static void EvaluateItem( IExpressionEvaluatorFacade evaluator,IReportExpression expressionItem)
		{
			string expr = String.Empty;
			if (expressionItem != null)
			{
				if (!String.IsNullOrEmpty(expressionItem.Expression)) {
					expr = expressionItem.Expression;
				} else {
					expr = expressionItem.Text; 
				}
			}

			expressionItem.Text = evaluator.Evaluate(expr);
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
					EvaluateItem(evaluator,expressionItem);
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
	}
}
