/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 21.09.2004
 * Time: 13:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

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
	/// Description of WindowStateCondition.
	/// </summary>
	public class ActiveWindowStateAuswerter : IAuswerter
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
