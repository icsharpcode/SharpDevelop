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
			if (dataNavigator == null) {
				throw new ArgumentNullException("dataNavigator");
			}
			singlePage.IDataNavigator = dataNavigator;
			IExpressionEvaluatorFacade evaluatorFacade = new ExpressionEvaluatorFacade(singlePage);
			return evaluatorFacade;
		}
		
		
		public static void EvaluateRow(IExpressionEvaluatorFacade evaluator,ExporterCollection row)
		{
			try {
				foreach (BaseExportColumn column in row) {
					var container = column as IExportContainer;
					if (container != null) {
						EvaluateRow(evaluator,container.Items);
					}
					ExportText textItem = column as ExportText;
					if (textItem != null) {
//						if (textItem.Text.StartsWith("=",StringComparison.InvariantCulture)) {
						////							Console.WriteLine(textItem.Text);
//						}
						textItem.Text = evaluator.Evaluate(textItem.Text);
					}
				}
			} catch (Exception) {
				throw ;
			}
		}
		
	}
}
