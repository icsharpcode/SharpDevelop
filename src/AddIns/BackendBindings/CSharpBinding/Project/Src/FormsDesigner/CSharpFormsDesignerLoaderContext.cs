// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.FormsDesigner;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using CSharpBinding.Parser;

namespace CSharpBinding.FormsDesigner
{
	class CSharpFormsDesignerLoaderContext : ICSharpDesignerLoaderContext
	{
		readonly FormsDesignerViewContent viewContent;

		public CSharpFormsDesignerLoaderContext(FormsDesignerViewContent viewContent)
		{
			this.viewContent = viewContent;
		}
		
		public IDocument PrimaryFileDocument {
			get {
				return viewContent.PrimaryFileDocument;
			}
		}
		
		public IDocument DesignerCodeFileDocument {
			get {
				return viewContent.DesignerCodeFileDocument;
			}
		}
		
		public CSharpFullParseInformation GetPrimaryFileParseInformation()
		{
			return SD.ParserService.Parse(viewContent.PrimaryFileName, viewContent.PrimaryFileDocument)
				as CSharpFullParseInformation;
		}
		
		public ICompilation GetCompilation()
		{
			return SD.ParserService.GetCompilationForFile(viewContent.PrimaryFileName);
		}
		
		public IDocument GetDocument(FileName fileName)
		{
			foreach (var pair in viewContent.SourceFiles) {
				if (pair.Key.FileName == fileName)
					return pair.Value;
			}
			throw new InvalidOperationException("Designer file not found");
		}
		
		public void ShowSourceCode(int lineNumber = 0)
		{
			viewContent.ShowSourceCode(lineNumber);
		}
	}
}


