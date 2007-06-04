// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class CtrlSpaceCompletionDataProvider : CodeCompletionDataProvider
	{
		public CtrlSpaceCompletionDataProvider()
		{
		}
		
		public CtrlSpaceCompletionDataProvider(ExpressionContext overrideContext)
		{
			this.overrideContext = overrideContext;
		}
		
		bool allowCompleteExistingExpression;
		
		/// <summary>
		/// Gets/Sets whether completing an old expression is allowed.
		/// You have to set this property to true to let the provider run FindExpression, when
		/// set to false it will use ExpressionContext.Default (unless the constructor with "overrideContext" was used).
		/// </summary>
		public bool AllowCompleteExistingExpression {
			get { return allowCompleteExistingExpression; }
			set { allowCompleteExistingExpression = value; }
		}
		
		protected override void GenerateCompletionData(TextArea textArea, char charTyped)
		{
			#if DEBUG
			if (DebugMode) {
				Debugger.Break();
			}
			#endif
			
			if (!allowCompleteExistingExpression) {
				preSelection = "";
				if (charTyped != '\0') {
					preSelection = null;
				}
				ExpressionContext context = overrideContext ?? ExpressionContext.Default;
				AddResolveResults(ParserService.CtrlSpace(caretLineNumber, caretColumn, fileName, textArea.Document.TextContent, context), context);
				return;
			}
			
			ExpressionResult expressionResult = GetExpression(textArea);
			LoggingService.Debug("Ctrl-Space got expression " + expressionResult.ToString());
			string expression = expressionResult.Expression;
			preSelection = null;
			if (expression == null || expression.Length == 0) {
				preSelection = "";
				if (charTyped != '\0') {
					preSelection = null;
				}
				AddResolveResults(ParserService.CtrlSpace(caretLineNumber, caretColumn, fileName, textArea.Document.TextContent, expressionResult.Context), expressionResult.Context);
				return;
			}
			
			int idx = expression.LastIndexOf('.');
			if (idx > 0) {
				preSelection = expression.Substring(idx + 1);
				expressionResult.Expression = expression.Substring(0, idx);
				if (charTyped != '\0') {
					preSelection = null;
				}
				GenerateCompletionData(textArea, expressionResult);
			} else {
				preSelection = expression;
				if (charTyped != '\0') {
					preSelection = null;
				}
				ArrayList results = ParserService.CtrlSpace(caretLineNumber, caretColumn, fileName, textArea.Document.TextContent, expressionResult.Context);
				AddResolveResults(results, expressionResult.Context);
			}
		}
	}
}
