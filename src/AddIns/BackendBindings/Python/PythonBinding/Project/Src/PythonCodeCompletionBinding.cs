// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

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
		public override bool HandleKeyword(SharpDevelopTextAreaControl editor, string word)
		{
			if (word != null) {
				switch (word.ToLowerInvariant()) {
					case "import":
					case "from":
						ICompletionDataProvider dataProvider = CreateCompletionDataProvider();
						ShowCodeCompletionWindow(editor, dataProvider, ' ');
						return true;
				}
			}
			return false;
		}
		
		protected virtual ICompletionDataProvider CreateCompletionDataProvider()
		{
			return new CodeCompletionDataProvider();
		}
		
		/// <summary>
		/// Shows the code completion window.
		/// </summary>
		protected virtual void ShowCodeCompletionWindow(SharpDevelopTextAreaControl textAreaControl, ICompletionDataProvider completionDataProvider, char ch)
		{
			textAreaControl.ShowCompletionWindow(completionDataProvider, ch);
		}
	}
}
