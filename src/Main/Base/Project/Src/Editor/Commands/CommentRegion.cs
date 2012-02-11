// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui;
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	/// <summary>
	/// Description of CommentRegion
	/// </summary>
	public class CommentRegion : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			
			if (provider == null)
				return;
			
			using (provider.TextEditor.Document.OpenUndoGroup())
				provider.TextEditor.Language.FormattingStrategy.SurroundSelectionWithComment(provider.TextEditor);
		}
	}
}
