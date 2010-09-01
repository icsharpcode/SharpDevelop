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

namespace RubyBinding.Tests.Utils
{
	/// <summary>
	/// Mock implementation of the IEditable and IViewContent.
	/// </summary>
	public class MockEditableViewContent : MockViewContent, IEditable, ITextEditorProvider
	{
		public MockTextEditor MockTextEditor = new MockTextEditor();
		
		public MockEditableViewContent()
		{
			Text = String.Empty;
		}
		
		public string Text { get; set; }
		
		public ITextBuffer CreateSnapshot()
		{
			return new StringTextBuffer(Text);
		}
		
		public ITextEditorOptions TextEditorOptions {
			get { return MockTextEditor.Options; }
		}
		
		public MockTextEditorOptions MockTextEditorOptions {
			get { return MockTextEditor.MockTextEditorOptions; }
			set { MockTextEditor.MockTextEditorOptions = value; }
		}
		
		public ITextEditor TextEditor {
			get { return MockTextEditor; }
		}
		
		public IDocument GetDocumentForFile(OpenedFile file)
		{
			throw new NotImplementedException();
		}
	}
}
