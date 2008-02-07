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
using ICSharpCode.TextEditor.Gui.CompletionWindow;

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
		
		/// <summary>
		/// Gets/Sets whether code templates should be included in code completion.
		/// </summary>
		public bool ShowTemplates { get; set; }
		
		void AddTemplates(TextArea textArea, char charTyped)
		{
			if (!ShowTemplates)
				return;
			ICompletionData suggestedData = DefaultIndex >= 0 ? completionData[DefaultIndex] : null;
			ICompletionData[] templateCompletionData = new TemplateCompletionDataProvider().GenerateCompletionData(fileName, textArea, charTyped);
			if (templateCompletionData == null || templateCompletionData.Length == 0)
				return;
			for (int i = 0; i < completionData.Count; i++) {
				if (completionData[i].ImageIndex == ClassBrowserIconService.KeywordIndex) {
					string text = completionData[i].Text;
					for (int j = 0; j < templateCompletionData.Length; j++) {
						if (templateCompletionData[j] != null && templateCompletionData[j].Text == text) {
							// replace keyword with template
							completionData[i] = templateCompletionData[j];
							templateCompletionData[j] = null;
						}
					}
				}
			}
			// add non-keyword code templates
			for (int j = 0; j < templateCompletionData.Length; j++) {
				if (templateCompletionData[j] != null)
					completionData.Add(templateCompletionData[j]);
			}
			if (suggestedData != null) {
				completionData.Sort(DefaultCompletionData.Compare);
				DefaultIndex = completionData.IndexOf(suggestedData);
			}
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
				AddTemplates(textArea, charTyped);
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
				AddTemplates(textArea, charTyped);
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
				AddTemplates(textArea, charTyped);
			}
		}
	}
}
