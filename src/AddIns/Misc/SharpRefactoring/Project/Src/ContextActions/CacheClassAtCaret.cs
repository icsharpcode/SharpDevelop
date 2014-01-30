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
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor.Commands;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// Caches information about the class at the caret in the editor.
	/// Used by ContextActions.
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
				return IsCaretAtClassDeclaration && Class.IsUserCode() && !IsClassReadOnly && !IsClassFileNameCorrect && IsCorrectClassFileNameAvailable;
			}
		}
		
		public bool IsCreateDerivedClassAvailable
		{
			get {
				return IsCaretAtClassDeclaration && Class.IsUserCode() && !Class.IsStatic && !Class.IsSealed;
			}
		}
		
		public void Initialize(EditorContext context)
		{
			// class at caret (either declaration of usage)
			this.Class = GetClass(context.CurrentSymbol);
			if (this.Class == null)
				return;
			var c = this.Class;
			
			var classDecls = context.GetClassDeclarationsOnCurrentLine().ToList();
			this.IsCaretAtClassDeclaration = classDecls.Count == 1 && (classDecls[0].FullyQualifiedName == c.FullyQualifiedName);
			
			this.IsClassFileNameCorrect = (c.IsInnerClass() || (!c.IsUserCode()) ||
			                               c.Name.Equals(Path.GetFileNameWithoutExtension(c.CompilationUnit.FileName), StringComparison.OrdinalIgnoreCase));
			
			if (string.IsNullOrEmpty(c.Name) || c.CompilationUnit == null || string.IsNullOrEmpty(c.CompilationUnit.FileName)) {
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
			if (c == null)
				return null;
			c = c.ProjectContent.GetClass(c.FullyQualifiedName, c.TypeParameters.Count, c.ProjectContent.Language, GetClassOptions.LookForInnerClass);
			return ClassBookmarkSubmenuBuilder.GetCurrentPart(c);
		}
	}
}
