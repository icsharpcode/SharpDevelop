// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		[Obsolete("This property always returns false.")]
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
		System.Windows.Media.ImageSource Icon {
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
		
		/// <summary>
		/// Is called when the title of this window has changed.
		/// </summary>
		event EventHandler TitleChanged;
	}
}
