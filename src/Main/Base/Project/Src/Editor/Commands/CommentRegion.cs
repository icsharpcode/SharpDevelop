// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
			
			provider.TextEditor.FormattingStrategy.SurroundSelectionWithComment(provider.TextEditor);
		}
	}
}
