// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

namespace CSharpBinding
{
	public class CSharpCompletionBinding : DefaultCodeCompletionBinding
	{
		public CSharpCompletionBinding() : base(".cs")
		{
			this.EnableXmlCommentCompletion = true;
		}
		
		public override bool HandleKeyPress(SharpDevelopTextAreaControl editor, char ch)
		{
			if (!CheckExtension(editor))
				return false;
			if (ch == '(') {
				ExpressionContext context;
				switch (editor.GetWordBeforeCaret().Trim()) {
					case "for":
					case "lock":
						context = ExpressionContext.Default;
						break;
					case "using":
						context = ExpressionContext.TypeDerivingFrom(ReflectionReturnType.Disposable.GetUnderlyingClass(), false);
						break;
					case "catch":
						context = ExpressionContext.TypeDerivingFrom(ReflectionReturnType.Exception.GetUnderlyingClass(), false);
						break;
					case "foreach":
					case "typeof":
					case "sizeof":
					case "default":
						context = ExpressionContext.Type;
						break;
					default:
						context = null;
						break;
				}
				if (context != null) {
					editor.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(context), ' ');
					return true;
				}
			}
			return base.HandleKeyPress(editor, ch);
		}
		
		public override bool HandleKeyword(SharpDevelopTextAreaControl editor, string word)
		{
			// TODO: Assistance writing Methods/Fields/Properties/Events:
			// use public/static/etc. as keywords to display a list with other modifiers
			// and possible return types.
			switch (word) {
				case "using":
					// TODO: check if we are inside class/namespace
					editor.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.Namespace), ' ');
					return true;
				case "as":
				case "is":
					editor.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.Type), ' ');
					return true;
				case "new":
					editor.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.ConstructableType), ' ');
					return true;
				default:
					return base.HandleKeyword(editor, word);
			}
		}
	}
}
