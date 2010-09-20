// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	/// <summary>
	/// Description of FindReferences.
	/// </summary>
	public class FindReferences : SymbolUnderCaretMenuCommand
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
