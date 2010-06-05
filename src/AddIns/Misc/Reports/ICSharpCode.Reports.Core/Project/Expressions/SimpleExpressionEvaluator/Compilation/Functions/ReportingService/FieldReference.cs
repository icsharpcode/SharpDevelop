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
			ISinglePage p = context.ContextObject as SinglePage;
			Variable v = Arguments[0] as Variable;
			if (p.IDataNavigator.CurrentRow > -1) {
				AvailableFieldsCollection avc = p.IDataNavigator.AvailableFields;
				
				AbstractColumn item = avc.Find(v.VariableName.ToString());
				CurrentItemsCollection cic = p.IDataNavigator.GetDataRow();
				CurrentItem c = cic.Find(v.VariableName);
				return c.Value.ToString();
			}
			
			return v.VariableName ;
		}
		
		
		
		protected override void AggregateValue(object value, AggregationState aggregationState, params object[] args)
        {
//            var sum = aggregationState.GetValue<string>("value");
//            var nextVal = TypeNormalizer.EnsureType<string>(value);
//
//            aggregationState["value"] = sum + nextVal;
        }

        protected override string ExtractAggregateValue(AggregationState aggregationState)
        {
            return aggregationState.GetValue<string>("value");
        }
	}
}
