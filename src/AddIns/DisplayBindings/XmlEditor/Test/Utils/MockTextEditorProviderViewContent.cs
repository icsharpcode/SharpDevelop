// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace XmlEditor.Tests.Utils
{
	public class MockTextEditorProviderViewContent : MockViewContent, ITextEditorProvider
	{
		MockTextEditor textEditor = new MockTextEditor();
		
		public ITextEditor TextEditor {
			get { return textEditor; }
		}
		
		public MockTextEditor MockTextEditor {
			get { return textEditor; }
		}
		
		public IDocument GetDocumentForFile(OpenedFile file)
		{
			throw new NotImplementedException();
		}
	}
}
