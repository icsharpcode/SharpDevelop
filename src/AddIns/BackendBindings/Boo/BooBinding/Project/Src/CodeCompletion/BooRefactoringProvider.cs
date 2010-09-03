// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Dom.Refactoring;

namespace Grunwald.BooBinding.CodeCompletion
{
	public class BooRefactoringProvider : RefactoringProvider
	{
		public static readonly BooRefactoringProvider BooProvider = new BooRefactoringProvider();
		
		public override bool IsEnabledForFile(string fileName)
		{
			return ".boo".Equals(Path.GetExtension(fileName), StringComparison.OrdinalIgnoreCase);
		}
	}
}
