// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public class RefactoringProvider
	{
		/// <summary>
		/// A RefactoringProvider instance that supports no refactorings.
		/// </summary>
		public static readonly RefactoringProvider DummyProvider = new RefactoringProvider();
		
		protected RefactoringProvider() {}
		
		public virtual bool SupportsFindUnusedUsingDeclarations {
			get {
				return false;
			}
		}
		
		public virtual IList<IUsing> FindUnusedUsingDeclarations(string fileName, string fileContent, ICompilationUnit compilationUnit)
		{
			throw new NotSupportedException();
		}
	}
}
