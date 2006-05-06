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
	public abstract class RefactoringProvider
	{
		/// <summary>
		/// A RefactoringProvider instance that supports no refactorings.
		/// </summary>
		public static readonly RefactoringProvider DummyProvider = new DummyRefactoringProvider();
		
		protected RefactoringProvider() {}
		
		public abstract bool IsEnabledForFile(string fileName);
		
		private class DummyRefactoringProvider : RefactoringProvider
		{
			public override bool IsEnabledForFile(string fileName)
			{
				return false;
			}
		}
		
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
