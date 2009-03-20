// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop;
using System;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Wraps the CodeEditor class to provide the ITextEditor interface.
	/// </summary>
	public class CodeEditorAdapter : AvalonEditTextEditorAdapter
	{
		readonly CodeEditor codeEditor;
		
		public CodeEditorAdapter(CodeEditor codeEditor) : base(codeEditor)
		{
			if (codeEditor == null)
				throw new ArgumentNullException("codeEditor");
			this.codeEditor = codeEditor;
		}
		
		public override string FileName {
			get { return codeEditor.FileName; }
		}
		
		protected override CompletionWindow CreateCompletionWindow(ICompletionItemList data)
		{
			CompletionWindow window = base.CreateCompletionWindow(data);
			codeEditor.NotifyCompletionWindowOpened(window);
			return window;
		}
	}
}
