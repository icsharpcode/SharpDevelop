// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
