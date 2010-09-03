// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor;
using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Gui;

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
