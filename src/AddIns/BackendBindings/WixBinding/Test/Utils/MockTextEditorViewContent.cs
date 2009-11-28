// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.TextEditor;

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
