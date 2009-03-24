// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop;
using System;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;

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
						new AttributesItemProvider(ExpressionFinder.BooAttributeContext.Instance).ShowCompletion(editor);
						return CodeCompletionKeyPressResult.Completed;
					}
					if (!char.IsWhiteSpace(c))
						break;
				}
			}
			return base.HandleKeyPress(editor, ch);
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
					new CtrlSpaceCompletionItemProvider(ExpressionContext.Importable).ShowCompletion(editor);
					return true;
				case "as":
				case "isa":
					if (!IsInComment(editor)) {
						new CtrlSpaceCompletionItemProvider(ExpressionContext.Type).ShowCompletion(editor);
						return true;
					}
					break;
			}
			return base.HandleKeyword(editor, word);
		}
	}
}
