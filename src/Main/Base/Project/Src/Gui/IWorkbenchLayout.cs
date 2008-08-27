// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
		void ShowPad(PadDescriptor content);
		
		/// <summary>
		/// Activates a pad (Show only makes it visible but Activate does
		/// bring it to foreground)
		/// </summary>
		void ActivatePad(PadDescriptor content);
		
		/// <summary>
		/// Hides a <see cref="IPadContent"/>.
		/// </summary>
		void HidePad(PadDescriptor content);
		
		/// <summary>
		/// Closes and disposes a <see cref="IPadContent"/>.
		/// </summary>
		void UnloadPad(PadDescriptor content);
		
		/// <summary>
		/// returns true, if padContent is visible;
		/// </summary>
		bool IsVisible(PadDescriptor padContent);
		
		/// <summary>
		/// Shows a new <see cref="IViewContent"/>.
		/// </summary>
		IWorkbenchWindow ShowView(IViewContent content);
		
		
		void LoadConfiguration();
		void StoreConfiguration();
	}
}
