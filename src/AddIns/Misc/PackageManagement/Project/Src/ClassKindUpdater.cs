// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;

namespace ICSharpCode.PackageManagement
{
	public class ClassKindUpdater : IClassKindUpdater
	{
		public ClassKindUpdater(IClass c)
			: this(c, new DocumentLoader())
		{
		}
		
		public ClassKindUpdater(IClass c, IDocumentLoader documentLoader)
		{
			this.Class = c;
			this.DocumentLoader = documentLoader;
		}
		
		IClass Class { get; set; }
		IDocumentLoader DocumentLoader { get; set; }
		IRefactoringDocument Document { get; set; }
		
		public void MakeClassPartial()
		{
			if (Class.IsPartial)
				return;
			
			OpenFileContainingClass();
			int offset = GetPartialKeywordInsertOffset();
			InsertPartialKeyword(offset);
		}
		
		void OpenFileContainingClass()
		{
			Document = DocumentLoader.LoadRefactoringDocument(Class.CompilationUnit.FileName);
		}
		
		int GetPartialKeywordInsertOffset()
		{
			IRefactoringDocumentLine line = Document.GetLine(Class.Region.BeginLine);
			int offset = line.Text.IndexOf(" class", StringComparison.OrdinalIgnoreCase);
			if (offset >= 0) {
				return offset + line.Offset + 1;
			}
			throw new ApplicationException("Unable to find 'class' declaration.");
		}
		
		void InsertPartialKeyword(int offset)
		{
			string partialKeyword = GetLanguageSpecificPartialKeyword();
			Document.Insert(offset, partialKeyword + " ");
		}
		
		string GetLanguageSpecificPartialKeyword()
		{
			if (Class.ProjectContent.Language == LanguageProperties.VBNet) {
				return "Partial";
			}
			return "partial";
		}
	}
}
