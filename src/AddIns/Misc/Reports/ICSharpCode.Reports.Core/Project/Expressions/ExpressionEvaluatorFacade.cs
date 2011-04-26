// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
				if (EvaluationHelper.CanEvaluate(expression)) {
					IExpression compiled = compiler.CompileExpression<string>(expression);
					
					this.context.ContextObject = this.SinglePage;
					if (compiled != null) {
						return (compiled.Evaluate(context)).ToString();
					}
				}
			} catch (Exception e) {
				expression = e.Message;
				WriteLogMessage(e);
			}
			
			return expression;
		}

		static void WriteLogMessage(Exception e)
		{
			Console.WriteLine("");
			Console.WriteLine("ExpressionEvaluatorFacade.Evaluate");
			Console.WriteLine(e.Message);
			Console.WriteLine("");
		}
		
		public string Evaluate (string expression, object row)
		{
			try {
				if (EvaluationHelper.CanEvaluate(expression)) {
					IExpression compiled = compiler.CompileExpression<string>(expression);
					
					this.context.ContextObject =row;
					if (compiled != null) {
						return (compiled.Evaluate(context)).ToString();
					}
				}
			} catch (Exception e) {
				expression = e.Message;
				WriteLogMessage(e);
			}
			
			return expression;
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
