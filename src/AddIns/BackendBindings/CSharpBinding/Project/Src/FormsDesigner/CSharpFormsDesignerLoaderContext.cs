// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.FormsDesigner;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
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
	}
}


