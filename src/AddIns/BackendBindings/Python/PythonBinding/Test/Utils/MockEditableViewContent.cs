// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Mock implementation of the IEditable and IViewContent.
	/// </summary>
	public class MockEditableViewContent : MockViewContent, IEditable, ITextEditorProvider
	{
		MockTextEditor textEditor = new MockTextEditor();
		string text = String.Empty;
		
		public MockEditableViewContent()
		{
		}
		
		public string Text {
			get { return text; }
			set { text = value; }
		}		
		
		public ITextBuffer CreateSnapshot()
		{
			return new StringTextBuffer(text);
		}
		
		public ITextEditorOptions TextEditorOptions {
			get { return textEditor.Options; }
			set { textEditor.Options = value; }
		}
		
		public ITextEditor TextEditor {
			get { return textEditor; }
		}
		
		public IDocument GetDocumentForFile(OpenedFile file)
		{
			throw new NotImplementedException();
		}
	}
}
