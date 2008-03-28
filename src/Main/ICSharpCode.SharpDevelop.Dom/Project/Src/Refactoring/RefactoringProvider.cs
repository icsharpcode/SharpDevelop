// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom.Refactoring
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
		
		public virtual IList<IUsing> FindUnusedUsingDeclarations(IDomProgressMonitor progressMonitor, string fileName, string fileContent, ICompilationUnit compilationUnit)
		{
			throw new NotSupportedException();
		}
		
		public virtual bool SupportsCreateNewFileLikeExisting {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Creates a new file that uses same header, usings and namespace like an existing file.
		/// </summary>
		/// <returns>the content for the new file,
		/// or null if an error occurred (error will be displayed to the user)</returns>
		/// <param name="existingFileContent">Content of the exisiting file</param>
		/// <param name="codeForNewType">Code to put in the new file.</param>
		public virtual string CreateNewFileLikeExisting(string existingFileContent, string codeForNewType)
		{
			throw new NotSupportedException();
		}
		
		
		public virtual bool SupportsGetFullCodeRangeForType {
			get {
				return false;
			}
		}
		
		public virtual DomRegion GetFullCodeRangeForType(string fileContent, IClass type)
		{
			throw new NotSupportedException();
		}
	}
}


