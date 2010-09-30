// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This is the basic interface to the workspace.
	/// </summary>
	public interface IWorkbench : IMementoCapable
	{
		/// <summary>
		/// The main window as IWin32Window.
		/// </summary>
		IWin32Window MainWin32Window { get; }
		
		/// <summary>
		/// Object for executing methods on the main thread.
		/// </summary>
		ISynchronizeInvoke SynchronizingObject { get; }
		
		/// <summary>
		/// The main window.
		/// </summary>
		Window MainWindow { get; }
		
		/// <summary>
		/// Gets the status bar.
		/// </summary>
		IStatusBarService StatusBar { get; }
		
		/// <summary>
		/// Gets/Sets whether the window is displayed in full-screen mode.
		/// </summary>
		bool FullScreen { get; set; }
		
		/// <summary>
		/// The title shown in the title bar.
		/// </summary>
		string Title {
			get;
			set;
		}
		
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
		object ActiveContent {
			get;
		}
		
		/// <summary>
		/// Is called, when the active content has changed.
		/// </summary>
		event EventHandler ActiveContentChanged;
		
		IWorkbenchLayout WorkbenchLayout {
			get;
			set;
		}
		
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
		/// Inserts a new <see cref="IPadContent"/> object in the workspace.
		/// </summary>
		void ShowPad(PadDescriptor content);
		
		/// <summary>
		/// Closes and disposes a <see cref="IPadContent"/>.
		/// </summary>
		void UnloadPad(PadDescriptor content);
		
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
		bool CloseAllSolutionViews();
		
		/// <summary>
		/// Is called, when a workbench view was opened
		/// </summary>
		/// <example>
		/// WorkbenchSingleton.WorkbenchCreated += delegate {
		/// 	WorkbenchSingleton.Workbench.ViewOpened += ...;
		/// };
		/// </example>
		event ViewContentEventHandler ViewOpened;
		
		/// <summary>
		/// Is called, when a workbench view was closed
		/// </summary>
		event ViewContentEventHandler ViewClosed;
	}
}
