// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Editor;
using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.TextEditor;

namespace RubyBinding.Tests.Utils
{
	/// <summary>
	/// A mock IViewContent implementation that also implements the
	/// ITextEditorControlProvider interface.
	/// </summary>
	public class MockTextEditorViewContent : MockViewContent, ITextEditorProvider
	{
		ITextEditor textEditor;
		
		public MockTextEditorViewContent()
		{
			textEditor = new ICSharpCode.SharpDevelop.Editor.AvalonEdit.AvalonEditTextEditorAdapter(
				new ICSharpCode.AvalonEdit.TextEditor()
			);
		}
		
		public ICSharpCode.SharpDevelop.Editor.ITextEditor TextEditor {
			get { return textEditor; }
		}
		
		public ICSharpCode.SharpDevelop.Editor.IDocument GetDocumentForFile(OpenedFile file)
		{
			throw new NotImplementedException();
		}
	}
}
