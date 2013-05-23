// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Workbench;

namespace WixBinding.Tests.Utils
{
	/// <summary>
	/// Mock IViewContent class that implements the ITextEditorProvider interface.
	/// </summary>
	public class MockTextEditorViewContent : MockViewContent, ITextEditorProvider
	{		
		public MockTextEditorViewContent()
		{
		}
		
		public ITextEditor TextEditor { get; set; }
		
		public IDocument GetDocumentForFile(OpenedFile file)
		{
			return null;
		}
	}
}
