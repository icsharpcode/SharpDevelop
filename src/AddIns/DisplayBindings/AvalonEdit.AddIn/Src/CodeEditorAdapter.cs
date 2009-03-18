// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

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
	}
}
