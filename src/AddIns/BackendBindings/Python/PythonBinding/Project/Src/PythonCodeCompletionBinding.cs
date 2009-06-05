// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Python code completion binding.
	/// </summary>
	public class PythonCodeCompletionBinding : DefaultCodeCompletionBinding
	{
		public PythonCodeCompletionBinding()
		{
		}
		
		/// <summary>
		/// Shows the code completion window if the keyword is handled.
		/// </summary>
		/// <param name="word">The keyword string.</param>
		/// <returns>true if the keyword is handled; otherwise false.</returns>
		public override bool HandleKeyword(ICSharpCode.SharpDevelop.Editor.ITextEditor editor, string word)
		{
			if (word != null) {
				switch (word.ToLowerInvariant()) {
					case "import":
					case "from":
						CtrlSpaceCompletionItemProvider dataProvider = CreateCtrlSpaceCompletionDataProvider(ExpressionContext.Importable);
						ShowCodeCompletionWindow(editor, dataProvider, ' ');
						return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Creates a CtrlSpaceCompletionItemProvider.
		/// </summary>
		protected virtual CtrlSpaceCompletionItemProvider CreateCtrlSpaceCompletionDataProvider(ExpressionContext expressionContext)
		{
			return new CtrlSpaceCompletionItemProvider(expressionContext);
		}
		
		/// <summary>
		/// Shows the code completion window.
		/// </summary>
		protected virtual void ShowCodeCompletionWindow(ICSharpCode.SharpDevelop.Editor.ITextEditor editor, AbstractCompletionItemProvider completionItemProvider, char ch)
		{
			completionItemProvider.ShowCompletion(editor);
		}
	}
}
