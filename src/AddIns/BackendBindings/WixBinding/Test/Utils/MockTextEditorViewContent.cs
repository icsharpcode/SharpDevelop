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
	public class MockTextEditorViewContent : MockViewContent
	{		
		public MockTextEditorViewContent()
		{
		}
		
		ITextEditor textEditor;
		
		public ITextEditor TextEditor {
			get { return textEditor ?? new MockTextEditor(); }
			set { textEditor = value; }
		}
		
		public IDocument GetDocumentForFile(OpenedFile file)
		{
			return null;
		}
	}
}
