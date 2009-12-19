// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
