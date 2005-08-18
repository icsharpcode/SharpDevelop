// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

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
			preSelection = null;
			if (fixedExpression.Expression == null)
				GenerateCompletionData(textArea, GetExpression(textArea));
			else
				GenerateCompletionData(textArea, fixedExpression);
		}
		
		#if DEBUG
		public bool DebugMode = false;
		#endif
		
		protected void GenerateCompletionData(TextArea textArea, ExpressionResult expressionResult)
		{
			if (expressionResult.Expression == null || expressionResult.Expression.Length == 0) {
				return;
			}
			if (LoggingService.IsDebugEnabled) {
				if (expressionResult.Context == ExpressionContext.Default)
					LoggingService.DebugFormatted("GenerateCompletionData for >>{0}<<", expressionResult.Expression);
				else
					LoggingService.DebugFormatted("GenerateCompletionData for >>{0}<<, context={1}", expressionResult.Expression, expressionResult.Context);
			}
			string textContent = textArea.Document.TextContent;
			#if DEBUG
			if (DebugMode) {
				Debugger.Break();
			}
			#endif
			AddResolveResults(ParserService.Resolve(expressionResult, caretLineNumber, caretColumn, fileName, textContent),
			                  expressionResult.Context);
		}
	}
}
