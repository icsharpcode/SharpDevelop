// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

namespace Grunwald.BooBinding.CodeCompletion
{
	public class CompletionBinding : DefaultCodeCompletionBinding
	{
		public CompletionBinding()
		{
			this.EnableXmlCommentCompletion = false;
		}
		
		public override CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			if (ch == '[') {
				int cursor = editor.Caret.Offset;
				for (int i = cursor - 1; i > 0; i--) {
					char c = editor.Document.GetCharAt(i);
					if (c == '\n' || c == '(' || c == ',') {
						// -> Attribute completion
						new AttributesItemProvider(
							new BooCtrlSpaceCompletionItemProvider(ExpressionFinder.BooAttributeContext.Instance)
						).ShowCompletion(editor);
						return CodeCompletionKeyPressResult.Completed;
					}
					if (!char.IsWhiteSpace(c))
						break;
				}
			}
			return base.HandleKeyPress(editor, ch);
		}
		
		public override bool CtrlSpace(ITextEditor editor)
		{
			BooCtrlSpaceCompletionItemProvider provider = new BooCtrlSpaceCompletionItemProvider();
			provider.AllowCompleteExistingExpression = true;
			provider.ShowCompletion(editor);
			return true;
		}
		
		bool IsInComment(ITextEditor editor)
		{
			ExpressionFinder ef = new ExpressionFinder(editor.FileName);
			int cursor = editor.Caret.Offset - 1;
			return ef.SimplifyCode(editor.Document.GetText(0, cursor + 1), cursor) == null;
		}
		
		public override bool HandleKeyword(ITextEditor editor, string word)
		{
			switch (word.ToLowerInvariant()) {
				case "import":
					new BooCtrlSpaceCompletionItemProvider(ExpressionContext.Importable).ShowCompletion(editor);
					return true;
				case "as":
				case "isa":
					if (!IsInComment(editor)) {
						new BooCtrlSpaceCompletionItemProvider(ExpressionContext.Type).ShowCompletion(editor);
						return true;
					}
					break;
			}
			return base.HandleKeyword(editor, word);
		}
	}
}
