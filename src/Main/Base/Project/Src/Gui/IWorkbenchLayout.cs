// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// The IWorkbenchLayout object is responsible for the layout of
	/// the workspace, it shows the contents, chooses the IWorkbenchWindow
	/// implementation etc. it could be attached/detached at the runtime
	/// to a workbench.
	/// </summary>
	public interface IWorkbenchLayout
	{
		/// <summary>
		/// The active workbench window.
		/// </summary>
		IWorkbenchWindow ActiveWorkbenchWindow {
			get;
		}
		
		/// <summary>
		/// Gets the open workbench windows.
		/// </summary>
		IList<IWorkbenchWindow> WorkbenchWindows {
			get;
		}
		
		/// <summary>
		/// The active content. This can be either a IViewContent or a IPadContent, depending on
		/// where the focus currently is.
		/// </summary>
		object ActiveContent {
			get;
		}
		
		event EventHandler ActiveWorkbenchWindowChanged;
		event EventHandler ActiveContentChanged;
		
		/// <summary>
		/// Attaches this layout manager to a workbench object.
		/// </summary>
		void Attach(IWorkbench workbench);
		
		/// <summary>
		/// Detaches this layout manager from the current workspace.
		/// </summary>
		void Detach();
		
		/// <summary>
		/// Shows a new <see cref="IPadContent"/>.
		/// </summary>
		void ShowPad(PadDescriptor padDescriptor);
		
		/// <summary>
		/// Activates a pad (Show only makes it visible but Activate does
		/// bring it to foreground)
		/// </summary>
		void ActivatePad(PadDescriptor padDescriptor);
		
		/// <summary>
		/// Hides a <see cref="IPadContent"/>.
		/// </summary>
		void HidePad(PadDescriptor padDescriptor);
		
		/// <summary>
		/// Closes and disposes a <see cref="IPadContent"/>.
		/// </summary>
		void UnloadPad(PadDescriptor padDescriptor);
		
		/// <summary>
		/// Returns true, if the pad header is visible (the pad content doesn't need to be visible).
		/// </summary>
		bool IsVisible(PadDescriptor padDescriptor);
		
		/// <summary>
		/// Shows a new <see cref="IViewContent"/> and optionally switches to it.
		/// </summary>
		IWorkbenchWindow ShowView(IViewContent content, bool switchToOpenedView);
		
		
		void LoadConfiguration();
		void StoreConfiguration();
	}
}
