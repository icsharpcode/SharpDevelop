// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
