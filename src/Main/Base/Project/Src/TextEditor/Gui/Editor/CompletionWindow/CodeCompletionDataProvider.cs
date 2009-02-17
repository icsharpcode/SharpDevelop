// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// Data provider for code completion.
	/// </summary>
	public class CodeCompletionDataProvider : AbstractCodeCompletionDataProvider
	{
		/// <summary>
		/// Initialize a CodeCompletionDataProvider that reads the expression from the text area.
		/// </summary>
		public CodeCompletionDataProvider()
		{
		}
		
		/// <summary>
		/// Initalize a CodeCompletionDataProvider with a fixed expression.
		/// </summary>
		public CodeCompletionDataProvider(ExpressionResult expression)
		{
			this.fixedExpression = expression;
		}
		
		ExpressionResult fixedExpression;
		
		protected override void GenerateCompletionData(TextArea textArea, char charTyped)
		{
			#if DEBUG
			if (DebugMode) {
				Debugger.Break();
			}
			#endif
			preSelection = null;
			if (fixedExpression.Expression == null)
				GenerateCompletionData(textArea, GetExpression(textArea));
			else
				GenerateCompletionData(textArea, fixedExpression);
		}
		
		#if DEBUG
		internal bool DebugMode = false;
		#endif
		
		protected void GenerateCompletionData(TextArea textArea, ExpressionResult expressionResult)
		{
			// allow empty string as expression (for VB 'With' statements)
			if (expressionResult.Expression == null) {
				return;
			}
			if (LoggingService.IsDebugEnabled) {
				if (expressionResult.Context == ExpressionContext.Default)
					LoggingService.DebugFormatted("GenerateCompletionData for >>{0}<<", expressionResult.Expression);
				else
					LoggingService.DebugFormatted("GenerateCompletionData for >>{0}<<, context={1}", expressionResult.Expression, expressionResult.Context);
			}
			string textContent = textArea.Document.TextContent;
			ResolveResult rr = Resolve(expressionResult, caretLineNumber, caretColumn, fileName, textContent);
			AddResolveResults(rr, expressionResult.Context);
		}
		
		protected virtual ResolveResult Resolve(ExpressionResult expressionResult,
		                                        int caretLineNumber, int caretColumn,
		                                        string fileName, string fileContent)
		{
			return ParserService.Resolve(expressionResult, caretLineNumber, caretColumn, fileName, fileContent);
		}
	}
}
