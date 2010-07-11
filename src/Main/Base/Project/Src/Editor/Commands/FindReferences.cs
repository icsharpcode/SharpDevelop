// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	/// <summary>
	/// Description of FindReferences.
	/// </summary>
	public class FindReferences : SymbolUnderCaretCommand
	{
		protected override void RunImpl(ITextEditor editor, int offset, ResolveResult symbol)
		{
			var classUnderCaret = GetClass(symbol);
			if (classUnderCaret != null) {
				FindReferencesAndRenameHelper.RunFindReferences(classUnderCaret);
				return;
			}
			var memberUnderCaret = GetMember(symbol);
			if (memberUnderCaret != null)
			{
				FindReferencesAndRenameHelper.RunFindReferences(memberUnderCaret);
				return;
			}
			if (symbol is LocalResolveResult) {
				FindReferencesAndRenameHelper.RunFindReferences((LocalResolveResult)symbol);
			}
		}
	}
}
