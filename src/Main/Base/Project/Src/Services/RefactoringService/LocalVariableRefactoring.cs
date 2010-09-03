// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public class FindLocalVariableReferencesCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			RefactoringMenuContext context = (RefactoringMenuContext)Owner;
			LocalResolveResult local = (LocalResolveResult)context.ResolveResult;
			FindReferencesAndRenameHelper.RunFindReferences(local);
		}
	}
	
	public class RenameLocalVariableCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			RefactoringMenuContext context = (RefactoringMenuContext)Owner;
			Run((LocalResolveResult)context.ResolveResult);
		}
		
		public static void Run(LocalResolveResult local)
		{
			string newName = MessageService.ShowInputBox("${res:SharpDevelop.Refactoring.Rename}", "${res:SharpDevelop.Refactoring.RenameMemberText}", local.VariableName);
			if (!FindReferencesAndRenameHelper.CheckName(newName, local.VariableName)) return;
			
			List<Reference> list = RefactoringService.FindReferences(local, null);
			if (list == null) return;
			FindReferencesAndRenameHelper.RenameReferences(list, newName);
		}
	}
}
