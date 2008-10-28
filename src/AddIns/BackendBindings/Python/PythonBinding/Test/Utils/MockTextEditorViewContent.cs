// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// A mock IViewContent implementation that also implements the
	/// ITextEditorControlProvider interface.
	/// </summary>
	public class MockTextEditorViewContent : MockViewContent, ITextEditorControlProvider
	{
		TextEditorControl textEditor;
		
		public MockTextEditorViewContent()
		{
			textEditor = new TextEditorControl();
		}
		
		public TextEditorControl TextEditorControl {
			get { return textEditor; }
		}
		
		public IDocument GetDocumentForFile(OpenedFile file)
		{
			throw new NotImplementedException();
		}
	}
}
