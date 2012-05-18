// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference	
{
	public class ActiveTextEditors : IActiveTextEditors
	{
		public string GetTextForOpenFile(string fileName)
		{
			ITextEditor textEditor = GetTextEditor(fileName);
			if (textEditor != null) {
				return textEditor.Document.Text;
			}
			return null;
		}
	
		ITextEditor GetTextEditor(string fileName)
		{
			IViewContent viewContent = FileService.GetOpenFile(fileName);
			var textEditorProvider = viewContent as ITextEditorProvider;
			if (textEditorProvider != null) {
				return textEditorProvider.TextEditor;
			}
			return null;
		}
		
		public void UpdateTextForOpenFile(string fileName, string text)
		{
			ITextEditor textEditor = GetTextEditor(fileName);
			if (textEditor != null) {
				using (IDisposable undoGroup = textEditor.Document.OpenUndoGroup()) {
					textEditor.Document.Text = text;
				}
			}
		}
		
		public bool IsFileOpen(string fileName)
		{
			return FileService.IsOpen(fileName);
		}
	}
}
