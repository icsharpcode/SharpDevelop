/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.06.2010
 * Time: 11:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.BaseClasses;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.Interfaces;
using SimpleExpressionEvaluator.Compilation.Functions.AggregateFunctions;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions.ReportingService
{
	[Tokens("Fields","fields")]
	public class FieldReference:AggregateFunction<string>
	{
		public FieldReference ()
		{
			
		}
		protected override int ExpectedArgumentCount {
			get { return 1; }
		}
		
		
		public override string Evaluate(SimpleExpressionEvaluator.Evaluation.IExpressionContext context)
		{
			ISinglePage singlePage = context.ContextObject as SinglePage;
			Variable variable = Arguments[0] as Variable;
			
			if (singlePage.IDataNavigator.CurrentRow > -1) {
				var dataRow = singlePage.IDataNavigator.GetDataRow;
				var item = dataRow.Find(variable.VariableName);
				
				string retval;
				if (item != null) {
					retval = item.Value.ToString();
				} else 
				{
					retval = GlobalValues.UnkownFunctionMessage(variable.VariableName);
						
					Console.WriteLine("");
					Console.WriteLine("ExpressionEvaluatorFacade.FieldReference");
					Console.WriteLine("Field<{0}> not found",variable.VariableName);
					Console.WriteLine("");
				}
				return retval;
			}
			return variable.VariableName ;
		}
		
		
		protected override void AggregateValue(object value, AggregationState aggregationState, params object[] args)
		{

		}

        protected override string ExtractAggregateValue(AggregationState aggregationState)
        {
            return aggregationState.GetValue<string>("value");
        }
	}
}
