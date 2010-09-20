// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	public abstract class CtrlSpaceCompletionItemProvider : CodeCompletionItemProvider
	{
		public CtrlSpaceCompletionItemProvider()
		{
		}
		
		public CtrlSpaceCompletionItemProvider(ExpressionContext overrideContext)
		{
			this.overrideContext = overrideContext;
		}
		
		ExpressionContext overrideContext;
		
		/// <summary>
		/// Gets/Sets whether completing an old expression is allowed.
		/// You have to set this property to true to let the provider run FindExpression, when
		/// set to false it will use ExpressionContext.Default (unless the constructor with "overrideContext" was used).
		/// </summary>
		public bool AllowCompleteExistingExpression { get; set; }
		
		/// <summary>
		/// Gets/Sets whether code templates should be included in code completion.
		/// </summary>
		public bool ShowTemplates { get; set; }
		
		void AddTemplates(ITextEditor editor, DefaultCompletionItemList list)
		{
			if (list == null)
				return;
			List<ICompletionItem> snippets = editor.GetSnippets().ToList();
			snippets.RemoveAll(item => !FitsToContext(item, list.Items));
			list.Items.RemoveAll(item => item.Image == ClassBrowserIconService.Keyword && snippets.Exists(i => i.Text == item.Text));
			list.Items.AddRange(snippets);
			list.SortItems();
		}
		
		bool FitsToContext(ICompletionItem item, List<ICompletionItem> list)
		{
			if (!(item is ISnippetCompletionItem))
				return false;
			
			var snippetItem = item as ISnippetCompletionItem;
			
			if (string.IsNullOrEmpty(snippetItem.Keyword))
				return true;
			
			return list.Any(x => x.Image == ClassBrowserIconService.Keyword
			                && x.Text == snippetItem.Keyword);
		}
		
		int preselectionLength;
		
		public override ICompletionItemList GenerateCompletionList(ITextEditor editor)
		{
			ICompletionItemList list = GenerateCompletionListCore(editor);
			if (ShowTemplates)
				AddTemplates(editor, list as DefaultCompletionItemList);
			return list;
		}
		
		ICompletionItemList GenerateCompletionListCore(ITextEditor editor)
		{
			preselectionLength = 0;
			if (!AllowCompleteExistingExpression) {
				ExpressionContext context = overrideContext ?? ExpressionContext.Default;
				var ctrlSpace = CtrlSpace(editor, context);
				return GenerateCompletionListForCompletionData(ctrlSpace, context);
			}
			
			ExpressionResult expressionResult = GetExpression(editor);
			LoggingService.Debug("Ctrl-Space got expression " + expressionResult.ToString());
			string expression = expressionResult.Expression;
			if (expression == null || expression.Length == 0) {
				var ctrlSpace = CtrlSpace(editor, expressionResult.Context);
				return GenerateCompletionListForCompletionData(ctrlSpace, expressionResult.Context);
			}
			
			int idx = expression.LastIndexOf('.');
			if (idx > 0) {
				preselectionLength = expression.Length - (idx + 1);
				expressionResult.Expression = expression.Substring(0, idx);
				return GenerateCompletionListForExpression(editor, expressionResult);
			} else {
				preselectionLength = expression.Length;
				List<ICompletionEntry> results = CtrlSpace(editor, expressionResult.Context);
				return GenerateCompletionListForCompletionData(results, expressionResult.Context);
			}
		}
		
		protected abstract List<ICompletionEntry> CtrlSpace(ITextEditor editor, ExpressionContext context);
		
		protected override void InitializeCompletionItemList(DefaultCompletionItemList list)
		{
			base.InitializeCompletionItemList(list);
			list.PreselectionLength = preselectionLength;
		}
	}
	
	public class NRefactoryCtrlSpaceCompletionItemProvider : CtrlSpaceCompletionItemProvider
	{
		LanguageProperties language;
		
		public NRefactoryCtrlSpaceCompletionItemProvider(LanguageProperties language)
		{
			if (language == null)
				throw new ArgumentNullException("language");
			this.language = language;
		}
		
		public NRefactoryCtrlSpaceCompletionItemProvider(LanguageProperties language, ExpressionContext overrideContext)
			: base(overrideContext)
		{
			if (language == null)
				throw new ArgumentNullException("language");
			this.language = language;
		}
		
		protected override DefaultCompletionItemList CreateCompletionItemList()
		{
			return new NRefactoryCompletionItemList() { ContainsItemsFromAllNamespaces = this.ShowItemsFromAllNamespaces };
		}
		
		protected override List<ICompletionEntry> CtrlSpace(ITextEditor editor, ExpressionContext context)
		{
			var resolver = new Dom.NRefactoryResolver.NRefactoryResolver(language);
			return resolver.CtrlSpace(
				editor.Caret.Line, editor.Caret.Column,
				ParserService.GetParseInformation(editor.FileName),
				editor.Document.Text,
				context, this.ShowItemsFromAllNamespaces);
		}
	}
}
