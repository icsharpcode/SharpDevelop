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

namespace ICSharpCode.SharpDevelop.Editor
{
	public class CtrlSpaceCompletionItemProvider : CodeCompletionItemProvider
	{
		public CtrlSpaceCompletionItemProvider()
		{
		}
		
		public CtrlSpaceCompletionItemProvider(ExpressionContext overrideContext)
		{
			this.overrideContext = overrideContext;
		}
		
		ExpressionContext overrideContext;
		
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
		
		// TODO: AVALONEDIT implement templates
		/*
		void AddTemplates(ITextEditor editor, char charTyped)
		{
			if (!ShowTemplates)
				return;
			ICompletionData suggestedData = DefaultIndex >= 0 ? completionData[DefaultIndex] : null;
			var templateCompletion = new TemplateCompletionItemProvider().GenerateCompletionList(editor);
			if (templateCompletion == null || templateCompletionData.Length == 0)
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
		 */
		
		int preselectionLength;
		
		public override ICompletionItemList GenerateCompletionList(ITextEditor editor)
		{
			preselectionLength = 0;
			if (!allowCompleteExistingExpression) {
				ExpressionContext context = overrideContext ?? ExpressionContext.Default;
				var ctrlSpace = ParserService.CtrlSpace(editor.Caret.Line, editor.Caret.Column, editor.FileName, editor.Document.Text, context);
				return GenerateCompletionListForCompletionData(ctrlSpace, context);
			}
			
			ExpressionResult expressionResult = GetExpression(editor);
			LoggingService.Debug("Ctrl-Space got expression " + expressionResult.ToString());
			string expression = expressionResult.Expression;
			if (expression == null || expression.Length == 0) {
				var ctrlSpace = ParserService.CtrlSpace(editor.Caret.Line, editor.Caret.Column, editor.FileName, editor.Document.Text, expressionResult.Context);
				return GenerateCompletionListForCompletionData(ctrlSpace, expressionResult.Context);
			}
			
			int idx = expression.LastIndexOf('.');
			if (idx > 0) {
				preselectionLength = expression.Length - (idx + 1);
				expressionResult.Expression = expression.Substring(0, idx);
				return GenerateCompletionListForExpression(editor, expressionResult);
			} else {
				preselectionLength = expression.Length;
				ArrayList results = ParserService.CtrlSpace(editor.Caret.Line, editor.Caret.Column, editor.FileName, editor.Document.Text, expressionResult.Context);
				return GenerateCompletionListForCompletionData(results, expressionResult.Context);
			}
		}
		
		protected override void InitializeCompletionItemList(DefaultCompletionItemList list)
		{
			base.InitializeCompletionItemList(list);
			list.PreselectionLength = preselectionLength;
		}
	}
}
