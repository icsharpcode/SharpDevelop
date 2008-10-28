// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WixBinding.Tests.Utils
{
	/// <summary>
	/// Mock IViewContent class that implements the ITextEditorControlProvider interface.
	/// </summary>
	public class MockTextEditorViewContent : MockViewContent, ITextEditorControlProvider
	{		
		public MockTextEditorViewContent()
		{
		}
	
		#region ITextEditorControlProvider

		public TextEditorControl TextEditorControl {
			get {
				return null;
			}
		}
		
		public IDocument GetDocumentForFile(OpenedFile file)
		{
			return null;
		}
				
		#endregion
	}
}
