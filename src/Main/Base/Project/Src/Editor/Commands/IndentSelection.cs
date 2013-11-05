// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	/// <summary>
	/// Description of IndentSelection
	/// </summary>
	public class IndentSelection : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			ITextEditor editor = SD.GetActiveViewContentService<ITextEditor>();
			if (editor == null)
				return;
			
			int beginLine = 1;
			int endLine = editor.Document.LineCount;
			
			if (editor.SelectionLength != 0) {
				beginLine = editor.Document.GetLineByOffset(editor.SelectionStart).LineNumber;
				endLine = editor.Document.GetLineByOffset(editor.SelectionStart + editor.SelectionLength).LineNumber;
			}
			
			using (editor.Document.OpenUndoGroup())
				editor.Language.FormattingStrategy.IndentLines(editor, beginLine, endLine);
		}
	}
}
