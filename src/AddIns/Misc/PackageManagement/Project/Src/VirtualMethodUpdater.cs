// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;

namespace ICSharpCode.PackageManagement
{
	public class VirtualMethodUpdater : IVirtualMethodUpdater
	{
		public VirtualMethodUpdater(IMethod method)
			: this(method, new DocumentLoader())
		{
		}
		
		public VirtualMethodUpdater(IMethod method, IDocumentLoader documentLoader)
		{
			this.Method = method;
			this.DocumentLoader = documentLoader;
		}
		
		IMethod Method { get; set; }
		IDocumentLoader DocumentLoader { get; set; }
		IRefactoringDocument Document { get; set; }
		
		public void MakeMethodVirtual()
		{
			if (Method.IsVirtual)
				return;
			
			OpenFileContainingMethod();
			int offset = GetVirtualKeywordInsertOffset();
			InsertVirtualKeyword(offset);
		}
		
		void OpenFileContainingMethod()
		{
			Document = DocumentLoader.LoadRefactoringDocument(Method.CompilationUnit.FileName);
		}
		
		int GetVirtualKeywordInsertOffset()
		{
			IRefactoringDocumentLine line = Document.GetLine(Method.Region.BeginLine);
			int offset = line.Text.IndexOf("public ", StringComparison.OrdinalIgnoreCase);
			if (offset >= 0) {
				int publicKeywordLength = 6;
				return offset + line.Offset + publicKeywordLength + 1;
			}
			throw new ApplicationException("Unable to find 'method' declaration.");
		}
		
		void InsertVirtualKeyword(int offset)
		{
			string virtualKeyword = GetLanguageSpecificVirtualKeyword();
			Document.Insert(offset, virtualKeyword + " ");
		}
		
		string GetLanguageSpecificVirtualKeyword()
		{
			if (Method.ProjectContent.Language == LanguageProperties.VBNet) {
				return "Overridable";
			}
			return "virtual";
		}
	}
}
