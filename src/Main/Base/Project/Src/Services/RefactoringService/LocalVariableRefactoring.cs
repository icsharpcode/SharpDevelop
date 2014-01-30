// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
/*
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
*/
