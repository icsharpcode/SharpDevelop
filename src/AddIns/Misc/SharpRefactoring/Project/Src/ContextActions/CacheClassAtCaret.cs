// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor.Commands;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// Description of CacheClassAtCaret.
	/// </summary>
	public class CacheClassAtCaret : IContextActionCache
	{
		public IClass Class { get; private set; }
		
		public bool IsClassFileNameCorrect { get; private set; }
		
		public string CorrectClassFileName { get; private set; }
		
		public bool IsCorrectClassFileNameAvailable { get; private set; }
		
		public bool IsClassReadOnly { get; private set; }
		
		public bool IsCaretAtClassDeclaration { get; private set; }
		
		public bool IsFixClassFileNameAvailable
		{
			get {
				return !IsClassReadOnly && IsCaretAtClassDeclaration && !IsClassFileNameCorrect && IsCorrectClassFileNameAvailable;
			}
		}
		
		public void Initialize(EditorContext context)
		{
			this.Class = GetClass(context.CurrentSymbol);
			if (this.Class == null)
				return;
			var c = this.Class;
			
			// TODO cache
			var classDecls = context.GetClassDeclarationsOnCurrentLine().ToList();
			this.IsCaretAtClassDeclaration = classDecls.Count == 1 && (classDecls[0].FullyQualifiedName == c.FullyQualifiedName);
			
			this.IsClassFileNameCorrect = (c.IsInnerClass() || (!c.IsUserCode()) ||
			                               c.Name.Equals(Path.GetFileNameWithoutExtension(c.CompilationUnit.FileName), StringComparison.OrdinalIgnoreCase));
			
			if (string.IsNullOrEmpty(c.CompilationUnit.FileName)) {
				// Cannot get path
				this.CorrectClassFileName = null;
				this.IsCorrectClassFileNameAvailable = false;
				return;
			}
			this.CorrectClassFileName = Path.Combine(Path.GetDirectoryName(c.CompilationUnit.FileName),
			                                    c.Name + Path.GetExtension(c.CompilationUnit.FileName));
			
			this.IsCorrectClassFileNameAvailable = (FileUtility.IsValidPath(CorrectClassFileName)
			                                        && Path.IsPathRooted(CorrectClassFileName)
			                                        && !File.Exists(CorrectClassFileName));
			
			this.IsClassReadOnly = FindReferencesAndRenameHelper.IsReadOnly(this.Class);
			
		}
		
		IClass GetClass(ResolveResult currentSymbol)
		{
			if (currentSymbol == null || currentSymbol.ResolvedType == null)
				return null;
			IClass c = currentSymbol.ResolvedType.GetUnderlyingClass();
			c = c.ProjectContent.GetClass(c.FullyQualifiedName, c.TypeParameters.Count, c.ProjectContent.Language, GetClassOptions.LookForInnerClass);
			return ClassBookmarkSubmenuBuilder.GetCurrentPart(c);
		}
	}
}
