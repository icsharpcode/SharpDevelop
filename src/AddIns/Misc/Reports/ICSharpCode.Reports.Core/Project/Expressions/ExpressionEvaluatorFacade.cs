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
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.Interfaces;
using SimpleExpressionEvaluator;
using SimpleExpressionEvaluator.Evaluation;
using SimpleExpressionEvaluator.Utilities;

namespace ICSharpCode.Reports.Expressions.ReportingLanguage
{
	/// <summary>
	/// Description of ExpressionEvaluatorFassade.
	/// </summary>
	public class ExpressionEvaluatorFacade:IExpressionEvaluatorFacade
	{
		private ReportingLanguageCompiler compiler;
		private ExpressionContext context;
		private IPageInfo singlePage;
		
		
		public ExpressionEvaluatorFacade(IPageInfo pageInfo)
		{
			compiler = new ReportingLanguageCompiler();
			this.context = new ExpressionContext(null);
			 context.ResolveUnknownVariable += VariableStore;
			context.ResolveMissingFunction += FunctionStore;
			SinglePage = pageInfo;
			compiler.SinglePage = pageInfo;
		}
		
		
		public string Evaluate (string expression)
		{
			try {
				string s = EvaluationHelper.ExtractExpressionPart(expression);
				string r = EvaluationHelper.ExtractResultPart(expression);
				if (s.Length > 0) {				    
					this.context.ContextObject = this.SinglePage ;
					return EvaluateExpression (s);
				}
				
			} catch (Exception e) {
				expression = e.Message;
				WriteLogMessage(e);
			}
			
			return expression;
		}

		public string Evaluate (string expression, object row)
		{
			try {
				string s = EvaluationHelper.ExtractExpressionPart(expression);
				if (s.Length > 0)
				{
					this.context.ContextObject = row;
					return EvaluateExpression (s);
				}
			} catch (Exception e) {
				expression = e.Message;
				WriteLogMessage(e);
			}
			return expression;
		}
		
		
		public void Evaluate (IReportExpression expressionItem)
		{
			if (expressionItem == null) {
				throw new ArgumentException("expressionItem");
			}
			string expr = String.Empty;
			if (!String.IsNullOrEmpty(expressionItem.Expression)) {
				expr = expressionItem.Expression;
			} else {
				expr = expressionItem.Text;
			}
			expressionItem.Text = Evaluate(expr);
		}
		
		
		string EvaluateExpression(string expression)
		{
			IExpression compiled = compiler.CompileExpression<string>(expression);
			if (compiled != null) {
				return (compiled.Evaluate(context)).ToString();
			}
			return expression;
		}

		
		static void WriteLogMessage(Exception e)
		{
			Console.WriteLine("-----LogMessage---------");
			Console.WriteLine("ExpressionEvaluatorFacade.Evaluate");
			Console.WriteLine(e.Message);
            Console.WriteLine(e.TargetSite);
			Console.WriteLine("");
		}
		
		
		private void FunctionStore (object sender,SimpleExpressionEvaluator.Evaluation.UnknownFunctionEventArgs e)
		{
			
			PropertyPath path = null;
			object searchObj = null;
			
			path = e.ContextObject.ParsePropertyPath(e.FunctionName);
			if (path != null) {
				searchObj = e.ContextObject;
			} else {
				throw new UnknownFunctionException(e.FunctionName);
			}
			e.Function = functionArgs => path.Evaluate(searchObj);
		}
		
		
		private void VariableStore (object sender,SimpleExpressionEvaluator.Evaluation.UnknownVariableEventArgs e)
		{
			PropertyPath path = this.singlePage.ParsePropertyPath(e.VariableName);
			if (path != null) {
				e.VariableValue = path.Evaluate(path);
			}
			
			// Look in Parametershash
			if (singlePage.ParameterHash.ContainsKey(e.VariableName))
			{
				try {
					e.VariableValue = singlePage.ParameterHash[e.VariableName].ToString();
				}
				catch (Exception)
				{
					e.VariableValue = String.Empty;
					Console.WriteLine("");
					Console.WriteLine("ExpressionEvaluatorFacade.VariableStore");
					Console.WriteLine("Replace Param <{0}> with String.Empty because no value is set",e.VariableName);
					Console.WriteLine("");
				}
			}
		}
		
		
		public IPageInfo SinglePage {
			get { return singlePage; }
			set {
				singlePage = value;
				this.compiler.SinglePage = singlePage;
			}
		}
	}
}
