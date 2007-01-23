// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
			IEditable editable = viewContent as IEditable;
			if (editable != null) {
				return new Document(viewContent, editable);
			} else {
				return null;
			}
		}
		
		IViewContent viewContent;
		IEditable editable;
		
		private Document(IViewContent viewContent, IEditable editable)
		{
			this.viewContent = viewContent;
			this.editable = editable;
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
			// IEditable implementor is responsible for thread-safety
			get {
				return editable.Text;
			}
			set {
				editable.Text = value;
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
