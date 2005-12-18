// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Core
{
	public enum WindowState {
		None     = 0,
		Untitled = 1,
		Dirty    = 2,
		ViewOnly = 4
	}
	
	/// <summary>
	/// Tests the window state of the active workbench window.
	/// </summary>
	public class ActiveWindowStateConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			if (WorkbenchSingleton.Workbench == null || 
			    WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null || 
			    WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent == null) {
				return false;
			}
			
			WindowState windowState   = condition.Properties.Get("windowstate", WindowState.None);
			WindowState nowindowState = condition.Properties.Get("nowindowstate", WindowState.None);
				
				
			bool isWindowStateOk = false;
			if (windowState != WindowState.None) {
				if ((windowState & WindowState.Dirty) > 0) {
					isWindowStateOk |= WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.IsDirty;
				} 
				if ((windowState & WindowState.Untitled) > 0) {
					isWindowStateOk |= WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.IsUntitled;
				}
				if ((windowState & WindowState.ViewOnly) > 0) {
					isWindowStateOk |= WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.IsViewOnly;
				}
			} else {
				isWindowStateOk = true;
			}
			
			if (nowindowState != WindowState.None) {
				if ((nowindowState & WindowState.Dirty) > 0) {
					isWindowStateOk &= !WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.IsDirty;
				}
				
				if ((nowindowState & WindowState.Untitled) > 0) {
					isWindowStateOk &= !WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.IsUntitled;
				}
				
				if ((nowindowState & WindowState.ViewOnly) > 0) {
					isWindowStateOk &= !WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.IsViewOnly;
				}
			}
			return isWindowStateOk;
		}
	}
}
