// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	/// <summary>
	/// Description of FindBaseClasses.
	/// </summary>
	public class FindBaseClasses : SymbolUnderCaretMenuCommand
	{
		protected override void RunImpl(ITextEditor editor, int offset, ResolveResult symbol)
		{
			var classUnderCaret = GetClass(symbol);
			if (classUnderCaret != null)
			{
				ContextActionsHelper.MakePopupWithBaseClasses(classUnderCaret).OpenAtCaretAndFocus();
				return;
			}
			MessageService.ShowError("${res:ICSharpCode.Refactoring.NoClassUnderCursorError}");
		}
	}
}
