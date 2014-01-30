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
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Workbench
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
