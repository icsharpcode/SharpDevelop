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
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			
			if (provider == null)
				return;
			
			int beginLine = 1;
			int endLine = provider.TextEditor.Document.TotalNumberOfLines;
			
			if (provider.TextEditor.SelectionLength != 0) {
				beginLine = provider.TextEditor.Document.GetLineForOffset(provider.TextEditor.SelectionStart).LineNumber;
				endLine = provider.TextEditor.Document.GetLineForOffset(provider.TextEditor.SelectionStart + provider.TextEditor.SelectionLength).LineNumber;
			}
			
			using (provider.TextEditor.Document.OpenUndoGroup())
				provider.TextEditor.Language.FormattingStrategy.IndentLines(provider.TextEditor, beginLine, endLine);
		}
	}
}
