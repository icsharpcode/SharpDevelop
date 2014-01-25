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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// Description of MoveClassToFile.
	/// </summary>
	public class MoveClassToFile : ContextAction
	{
		public CacheClassAtCaret ClassAtCaret
		{
			get { return this.Context.GetCached<CacheClassAtCaret>(); }
		}
		
		public override string Title {
			get {
				var fileName = Path.GetFileName(ClassAtCaret.CorrectClassFileName);
				return StringParser.Parse("${res:SharpDevelop.Refactoring.MoveClassToFile}", new StringTagPair("FileName", fileName));
			}
		}
		
		public override bool IsAvailable(EditorContext context)
		{
			if (ClassAtCaret.Class == null) return false;
			return (ClassAtCaret.IsFixClassFileNameAvailable && ClassAtCaret.Class.CompilationUnit.Classes.Count != 1);
		}
		
		public override void Execute(EditorContext context)
		{
			FindReferencesAndRenameHelper.MoveClassToFile(ClassAtCaret.Class, ClassAtCaret.CorrectClassFileName);
		}
	}
}
