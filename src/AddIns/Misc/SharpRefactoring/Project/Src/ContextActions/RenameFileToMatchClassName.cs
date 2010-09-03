// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// Description of RenameFileToMatchClassFileName.
	/// </summary>
	public class RenameFileToMatchClassName : ContextAction
	{
		public CacheClassAtCaret ClassAtCaret
		{
			get { return this.Context.GetCached<CacheClassAtCaret>(); }
		}
		
		public override string Title {
			get {
				var fileName = Path.GetFileName(ClassAtCaret.CorrectClassFileName);
				return StringParser.Parse("${res:SharpDevelop.Refactoring.RenameFileTo}", new StringTagPair("FileName", fileName));
			}
		}
		
		public override bool IsAvailable(EditorContext context)
		{
			if (ClassAtCaret.Class == null) return false;
			return (ClassAtCaret.IsFixClassFileNameAvailable && ClassAtCaret.Class.CompilationUnit.Classes.Count == 1);
		}
		
		public override void Execute(EditorContext context)
		{
			IProject project = (IProject)ClassAtCaret.Class.ProjectContent.Project;
			RefactoringHelpers.RenameFile(project, ClassAtCaret.Class.CompilationUnit.FileName, ClassAtCaret.CorrectClassFileName);
			if (project != null) {
				project.Save();
			}
		}
	}
}
