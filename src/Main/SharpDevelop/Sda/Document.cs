// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;
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
			if (viewContent == null)
				return null;
			ITextEditor editor = viewContent.GetService(typeof(ITextEditor)) as ITextEditor;
			if (editor != null) {
				return new Document(viewContent, editor.Document);
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
