// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public class FindLocalVariableReferencesCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			LocalResolveResult local = (LocalResolveResult)Owner;
			List<Reference> list = RefactoringService.FindReferences(local, null);
			FindReferencesAndRenameHelper.ShowAsSearchResults("References to " + local.Field.Name, list);
		}
	}
	
	public class RenameLocalVariableCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			LocalResolveResult local = (LocalResolveResult)Owner;
			string newName = MessageService.ShowInputBox("${res:SharpDevelop.Refactoring.Rename}", "${res:SharpDevelop.Refactoring.RenameMemberText}", local.Field.Name);
			if (!FindReferencesAndRenameHelper.CheckName(newName)) return;
			
			List<Reference> list = RefactoringService.FindReferences(local, null);
			if (list == null) return;
			FindReferencesAndRenameHelper.RenameReferences(list, newName);
		}
	}
}
