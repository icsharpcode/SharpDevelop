// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
		
		public override bool HandleKeyPress(SharpDevelopTextAreaControl editor, char ch)
		{
			if (ch == '[') {
				int cursor = editor.ActiveTextAreaControl.Caret.Offset;
				for (int i = cursor - 1; i > 0; i--) {
					char c = editor.Document.GetCharAt(i);
					if (c == '\n' || c == '(' || c == ',') {
						// -> Attribute completion
						editor.ShowCompletionWindow(new AttributesDataProvider(ExpressionFinder.BooAttributeContext.Instance), ch);
						return true;
					}
					if (!char.IsWhiteSpace(c))
						break;
				}
			}
			return base.HandleKeyPress(editor, ch);
		}
		
		bool IsInComment(SharpDevelopTextAreaControl editor)
		{
			ExpressionFinder ef = new ExpressionFinder(editor.FileName);
			int cursor = editor.ActiveTextAreaControl.Caret.Offset - 1;
			return ef.SimplifyCode(editor.Document.GetText(0, cursor + 1), cursor) == null;
		}
		
		public override bool HandleKeyword(SharpDevelopTextAreaControl editor, string word)
		{
			switch (word.ToLowerInvariant()) {
				case "import":
					editor.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.Importable), ' ');
					return true;
				case "as":
				case "isa":
					if (IsInComment(editor)) return false;
					editor.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.Type), ' ');
					return true;
				default:
					return base.HandleKeyword(editor, word);
			}
		}
	}
}
