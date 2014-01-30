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
