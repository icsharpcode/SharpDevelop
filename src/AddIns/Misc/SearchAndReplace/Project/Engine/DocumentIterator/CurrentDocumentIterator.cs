// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Editor;
using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;

namespace SearchAndReplace
{
	public class CurrentDocumentIterator : IDocumentIterator
	{
		bool      didRead = false;
		
		public CurrentDocumentIterator()
		{
			Reset();
		}
		
		public string CurrentFileName {
			get {
				if (!SearchReplaceUtilities.IsTextAreaSelected) {
					return null;
				}
				return WorkbenchSingleton.Workbench.ActiveViewContent.PrimaryFileName;
			}
		}
		
		public ProvidedDocumentInformation Current {
			get {
				ITextEditor textEditor = SearchReplaceUtilities.GetActiveTextEditor();
				if (textEditor != null)
					return new ProvidedDocumentInformation(textEditor.Document, CurrentFileName, textEditor);
				else
					return null;
			}
		}
		
		public bool MoveForward()
		{
			if (!SearchReplaceUtilities.IsTextAreaSelected) {
				return false;
			}
			if (didRead) {
				return false;
			}
			didRead = true;
			
			return true;
		}
		
		public bool MoveBackward()
		{
			return MoveForward();
		}
		
		public void Reset()
		{
			didRead = false;
		}
	}
}
