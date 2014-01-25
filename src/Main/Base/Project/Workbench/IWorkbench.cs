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
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// This is the basic interface to the workspace.
	/// </summary>
	[SDService("SD.Workbench")]
	public interface IWorkbench
	{
		/// <summary>
		/// The main window as IWin32Window.
		/// </summary>
		[Obsolete("Use SD.WinForms.MainWin32Window instead")]
		IWin32Window MainWin32Window { get; }
		
		/// <summary>
		/// The main window.
		/// </summary>
		Window MainWindow { get; }
		
		/// <summary>
		/// Gets/Sets whether the window is displayed in full-screen mode.
		/// </summary>
		bool FullScreen { get; set; }
		
		/// <summary>
		/// A collection in which all opened view contents (including all secondary view contents) are saved.
		/// </summary>
		ICollection<IViewContent> ViewContentCollection {
			get;
		}
		
		/// <summary>
		/// A collection in which all opened primary view contents are saved.
		/// </summary>
		ICollection<IViewContent> PrimaryViewContents {
			get;
		}
		
		/// <summary>
		/// A collection in which all active workspace windows are saved.
		/// </summary>
		IList<IWorkbenchWindow> WorkbenchWindowCollection {
			get;
		}
		
		/// <summary>
		/// A collection in which all active workspace windows are saved.
		/// </summary>
		IList<PadDescriptor> PadContentCollection {
			get;
		}
		
		/// <summary>
		/// The active workbench window.
		/// This is the window containing the active view content.
		/// </summary>
		IWorkbenchWindow ActiveWorkbenchWindow {
			get;
		}
		
		/// <summary>
		/// Is called, when the ActiveWorkbenchWindow property changes.
		/// </summary>
		event EventHandler ActiveWorkbenchWindowChanged;
		
		/// <summary>
		/// The active view content inside the active workbench window.
		/// </summary>
		IViewContent ActiveViewContent {
			get;
		}
		
		/// <summary>
		/// Is called, when the active view content has changed.
		/// </summary>
		event EventHandler ActiveViewContentChanged;
		
		/// <summary>
		/// The active content, depending on where the focus currently is.
		/// If a document is currently active, this will be equal to ActiveViewContent,
		/// if a pad has the focus, this property will return the IPadContent instance.
		/// </summary>
		IServiceProvider ActiveContent {
			get;
		}
		
		/// <summary>
		/// Is called, when the active content has changed.
		/// </summary>
		event EventHandler ActiveContentChanged;
		
		/// <summary>
		/// Gets whether SharpDevelop is the active application in Windows.
		/// </summary>
		bool IsActiveWindow {
			get;
		}
		
		/// <summary>
		/// Initializes the workbench.
		/// </summary>
		void Initialize();
		
		/// <summary>
		/// Inserts a new <see cref="IViewContent"/> object in the workspace and switches to the new view.
		/// </summary>
		void ShowView(IViewContent content);
		
		/// <summary>
		/// Inserts a new <see cref="IViewContent"/> object in the workspace.
		/// </summary>
		void ShowView(IViewContent content, bool switchToOpenedView);
		
		/// <summary>
		/// Activates the specified pad.
		/// </summary>
		void ActivatePad(PadDescriptor content);
		
		/// <summary>
		/// Returns a pad from a specific type.
		/// </summary>
		PadDescriptor GetPad(Type type);
		
		/// <summary>
		/// Closes all views inside the workbench.
		/// </summary>
		void CloseAllViews();
		
		/// <summary>
		/// 	Closes all views related to current solution.
		/// </summary>
		/// <returns>
		/// 	True if all views were closed properly, false if closing was aborted.
		/// </returns>
		bool CloseAllSolutionViews(bool force);
		
		/// <summary>
		/// Gets/Sets the name of the current layout configuration.
		/// Setting this property causes the current layout to be saved, and the specified layout to be loaded.
		/// </summary>
		string CurrentLayoutConfiguration { get; set; }
		
		/// <summary>
		/// Is called, when a workbench view was opened
		/// </summary>
		/// <example>
		/// WorkbenchSingleton.WorkbenchCreated += delegate {
		/// 	WorkbenchSingleton.Workbench.ViewOpened += ...;
		/// };
		/// </example>
		event EventHandler<ViewContentEventArgs> ViewOpened;
		
		/// <summary>
		/// Is called, when a workbench view was closed
		/// </summary>
		event EventHandler<ViewContentEventArgs> ViewClosed;
	}
}
