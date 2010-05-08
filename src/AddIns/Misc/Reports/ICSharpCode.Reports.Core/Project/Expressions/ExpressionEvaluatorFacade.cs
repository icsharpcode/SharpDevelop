/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 28.06.2009
 * Zeit: 18:54
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using ICSharpCode.Reports.Core.Interfaces;
using System;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Expressions.ReportingLanguage;
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
		private ISinglePage singlePage;
		 
		public ExpressionEvaluatorFacade()
		{
			compiler = new ReportingLanguageCompiler();
			this.context = new ExpressionContext(null);
			context.ResolveUnknownVariable += VariableStore;
			context.ResolveMissingFunction += FunctionStore;
		}
		
//		public string Evaluate (string expression)
//		{
//			IExpression compiled = compiler.CompileExpression<string>(expression);
//			this.context.ContextObject = this.SinglePage;
//			if (compiled != null) {
//				return (compiled.Evaluate(context)).ToString();
//			}
//			return expression;
//		}
		
		
		public string Evaluate (string expression)
		{
			if (!String.IsNullOrEmpty(expression)) {
				if (expression.StartsWith("=")) {
					IExpression compiled = compiler.CompileExpression<string>(expression);
					this.context.ContextObject = this.SinglePage;
					if (compiled != null) {
						return (compiled.Evaluate(context)).ToString();
					}
				}
				
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
				throw new InvalidOperationException("A function specified in the expression (" + e.FunctionName +
                                                ") does not exist.");
				
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
			if (singlePage.ParameterHash.ContainsKey(e.VariableName)) {
				e.VariableValue = singlePage.ParameterHash[e.VariableName].ToString();
			}
		}
		
		
		public ISinglePage SinglePage {
			get { return singlePage; }
			set {
				singlePage = value;
				this.compiler.SinglePage = singlePage;
			}
		}
	}
}
