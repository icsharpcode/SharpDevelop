// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui;
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	/// <summary>
	/// Surrounds the selected text in a comment
	/// </summary>
	public class CommentRegion : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			ITextEditor editor = SD.GetActiveViewContentService<ITextEditor>();
			if (editor == null)
				return;
			
			using (editor.Document.OpenUndoGroup())
				editor.Language.FormattingStrategy.SurroundSelectionWithComment(editor);
		}
	}
}
