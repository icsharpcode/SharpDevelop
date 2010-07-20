// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Dom;
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	public class GoToDefinition : SymbolUnderCaretMenuCommand
	{
		protected override void RunImpl(ITextEditor editor, int offset, ResolveResult symbol)
		{
			if (symbol == null)
				return;
			FilePosition pos = symbol.GetDefinitionPosition();
			if (pos.IsEmpty)
				return;
			try {
				if (pos.Position.IsEmpty)
					FileService.OpenFile(pos.FileName);
				else
					FileService.JumpToFilePosition(pos.FileName, pos.Line, pos.Column);
			} catch (Exception ex) {
				MessageService.ShowException(ex, "Error jumping to '" + pos.FileName + "'.");
			}
		}
	}
}
