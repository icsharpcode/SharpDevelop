// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor;
using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Sda
{
	/// <summary>
	/// Wraps a file opened in SharpDevelop.
	/// </summary>
	public sealed class Document : MarshalByRefObject
	{
		internal static Document FromWindow(IViewContent viewContent)
		{
			ITextEditorProvider editable = viewContent as ITextEditorProvider;
			if (editable != null) {
				return new Document(viewContent, editable.TextEditor.PrimaryView.Document);
			} else {
				return null;
			}
		}
		
		IViewContent viewContent;
		IDocument textDocument;
		
		private Document(IViewContent viewContent, IDocument textDocument)
		{
			this.viewContent = viewContent;
			this.textDocument = textDocument;
		}
		
		/// <summary>
		/// Gets the file name assigned to this document. Can be null.
		/// </summary>
		public string FileName {
			get {
				return viewContent.PrimaryFileName;
			}
		}
		
		/// <summary>
		/// Gets/Sets the text displayed in the document.
		/// </summary>
		public string Text {
			get {
				return textDocument.Text;
			}
			set {
				textDocument.Text = value;
			}
		}
		
		/// <summary>
		/// Gets if the document tab has been disposed because the document has been closed.
		/// </summary>
		public bool IsDisposed {
			get {
				return viewContent.IsDisposed;
			}
		}
		
		/// <summary>
		/// Closes the document.
		/// </summary>
		/// <param name="force">If true, the window is closed without giving the user
		/// a change to save changes if there were modifications.</param>
		/// <returns>true, if the window has been closed</returns>
		public bool Close(bool force)
		{
			return viewContent.WorkbenchWindow.CloseWindow(force);
		}
	}
}
