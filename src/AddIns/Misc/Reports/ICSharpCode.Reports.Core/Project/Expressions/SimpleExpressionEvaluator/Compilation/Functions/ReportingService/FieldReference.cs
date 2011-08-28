/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.06.2010
 * Time: 11:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
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
			Variable variable = Arguments[0] as Variable;
			string retval = string.Empty;
			
			ISinglePage singlePage = context.ContextObject as SinglePage;
			
			if (singlePage != null)
			{
				return ExtractValueFromSinglePage(ref variable, singlePage, ref retval);
			}
			
			DataRow row  = context.ContextObject as DataRow;
			if (row != null) {
				
				return ExtractValueFromDataRow(ref variable, retval, row);
			}
			return variable.VariableName ;
		}


		string ExtractValueFromSinglePage(ref Variable variable, ISinglePage singlePage, ref string retval)
		{
			if (singlePage.IDataNavigator.CurrentRow > -1)
			{
				try {
					var dataRow = singlePage.IDataNavigator.GetDataRow;
					var item = dataRow.Find(variable.VariableName);
					
					if (item != null) {
						retval = item.Value.ToString();
					} else {
						retval = GlobalValues.UnkownFunctionMessage(variable.VariableName);
						WriteLogmessage(variable);
					}
					
					return retval;
				} catch (Exception e) {
					Console.WriteLine ("Error in FieldReference.ExtractValueFromSinglePage");
					Console.WriteLine("IDataNavigator currentrow =  {0} count = {1}",singlePage.IDataNavigator.CurrentRow,singlePage.IDataNavigator.Count);
					throw e;
				}
			}
			return variable.VariableName;
		}

		
		string ExtractValueFromDataRow(ref Variable variable, string retval, DataRow row)
		{
			var item = row[variable.VariableName];
			if (item != null) {
				retval = item.ToString();
			} else {
				retval = GlobalValues.UnkownFunctionMessage(variable.VariableName);
				WriteLogmessage(variable);
			}
			return retval;
		}
		
		
		void WriteLogmessage(Variable variable)
		{
			Console.WriteLine("");
			Console.WriteLine("ExpressionEvaluatorFacade.FieldReference");
			Console.WriteLine("Field<{0}> not found",variable.VariableName);
			Console.WriteLine("");
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
