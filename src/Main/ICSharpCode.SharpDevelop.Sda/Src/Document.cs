/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 27.07.2006
 * Time: 21:54
 */

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Sda
{
	/// <summary>
	/// Wraps a file opened in SharpDevelop.
	/// </summary>
	public sealed class Document : MarshalByRefObject
	{
		internal static Document FromWindow(IWorkbenchWindow window)
		{
			if (window != null && window.ViewContent is IEditable) {
				return new Document(window, ((IEditable)window.ViewContent));
			} else {
				return null;
			}
		}
		
		IWorkbenchWindow window;
		IEditable editable;
		
		private Document(IWorkbenchWindow window, IEditable editable)
		{
			this.window = window;
			this.editable = editable;
		}
		
		/// <summary>
		/// Gets the file name assigned to this document. Can be null.
		/// </summary>
		public string FileName {
			get {
				return window.ViewContent.FileName;
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
				return window.IsDisposed;
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
			return window.CloseWindow(force);
		}
	}
}
