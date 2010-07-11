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
			if (symbol == null)
				return;
			if (symbol is TypeResolveResult) {
				var classUnderCaret = ((TypeResolveResult)symbol).ResolvedClass;
				if (classUnderCaret == null)
					return;
				FindReferencesAndRenameHelper.RunFindReferences(classUnderCaret);
			}
			if (symbol is MemberResolveResult) {
				IMember memberUnderCaret = ((MemberResolveResult)symbol).ResolvedMember as IMember;
				if (memberUnderCaret == null)
					return;
				FindReferencesAndRenameHelper.RunFindReferences(memberUnderCaret);
			}
		}
	}
}
