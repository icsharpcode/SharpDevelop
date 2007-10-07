// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// The IWorkbenchWindow is the basic interface to a window which
	/// shows a view (represented by the IViewContent object).
	/// </summary>
	public interface IWorkbenchWindow
	{
		/// <summary>
		/// The window title.
		/// </summary>
		string Title {
			get;
		}
		
		/// <summary>
		/// Gets if the workbench window has been disposed.
		/// </summary>
		bool IsDisposed {
			get;
		}
		
		/// <summary>
		/// Gets/Sets current view content which is shown inside this window.
		/// </summary>
		IViewContent ActiveViewContent {
			get;
			set;
		}
		
		/// <summary>
		/// Gets/Sets the icon of the view content.
		/// </summary>
		System.Drawing.Icon Icon {
			get;
			set;
		}
		
		/// <summary>
		/// Is raised when the ActiveViewContent property has changed.
		/// </summary>
		event EventHandler ActiveViewContentChanged;
		
		/// <summary>
		/// Gets the list of view contents displayed in this window
		/// </summary>
		IList<IViewContent> ViewContents {
			get;
		}
		
		/// <summary>
		/// Activates the view with the specified index.
		/// </summary>
		void SwitchView(int viewNumber);
		
		/// <summary>
		/// Closes the window, if force == true it closes the window
		/// without asking, even the content is dirty.
		/// </summary>
		/// <returns>true, if window is closed</returns>
		bool CloseWindow(bool force);
		
		/// <summary>
		/// Brings this window to front and sets the user focus to this
		/// window.
		/// </summary>
		void SelectWindow();
		
		void RedrawContent();
		
		/// <summary>
		/// Used internally:
		/// This method is called by the workbench to notify the window that is was selected.
		/// </summary>
		void OnWindowSelected(EventArgs e);
		
		/// <summary>
		/// Used internally:
		/// This method is called by the workbench to notify the window that is was deselected.
		/// </summary>
		void OnWindowDeselected(EventArgs e);
		
		/// <summary>
		/// Is called when the window is selected.
		/// </summary>
		event EventHandler WindowSelected;
		
		/// <summary>
		/// Is called when the window is deselected.
		/// </summary>
		event EventHandler WindowDeselected;
		
		/// <summary>
		/// Is called when the title of this window has changed.
		/// </summary>
		event EventHandler TitleChanged;
		
		/// <summary>
		/// Is called after the window closes.
		/// </summary>
		event EventHandler CloseEvent;
	}
}
