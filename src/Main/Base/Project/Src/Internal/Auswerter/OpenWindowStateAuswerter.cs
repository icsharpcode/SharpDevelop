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
	/// <summary>
	/// Description of WindowStateCondition.
	/// </summary>
	public class OpenWindowStateAuswerter : IAuswerter
	{
		WindowState windowState = WindowState.None;
		WindowState nowindowState = WindowState.None;
		
		bool IsStateOk(IWorkbenchWindow window)
		{
			if (window == null || window.ViewContent == null) {
				return false;
			}
			// use IWorkbenchWindow instead of IViewContent because maybe window info is needed in the future (for example: sub view content info.)
			bool isWindowStateOk = false;
			if (windowState != WindowState.None) {
				if ((windowState & WindowState.Dirty) > 0) {
					isWindowStateOk |= window.ViewContent.IsDirty;
				} 
				if ((windowState & WindowState.Untitled) > 0) {
					isWindowStateOk |= window.ViewContent.IsUntitled;
				}
				if ((windowState & WindowState.ViewOnly) > 0) {
					isWindowStateOk |= window.ViewContent.IsViewOnly;
				}
			} else {
				isWindowStateOk = true;
			}
			
			if (nowindowState != WindowState.None) {
				if ((nowindowState & WindowState.Dirty) > 0) {
					isWindowStateOk &= !window.ViewContent.IsDirty;
				}
				
				if ((nowindowState & WindowState.Untitled) > 0) {
					isWindowStateOk &= !window.ViewContent.IsUntitled;
				}
				
				if ((nowindowState & WindowState.ViewOnly) > 0) {
					isWindowStateOk &= !window.ViewContent.IsViewOnly;
				}
			}
			return isWindowStateOk;
		}
		
		public bool IsValid(object caller, Condition condition)
		{
			if (WorkbenchSingleton.Workbench == null) {
				return false;
			}
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent == null) {
				return false;
			}
			
			windowState   = condition.Properties.Get("windowstate", WindowState.None);
			nowindowState = condition.Properties.Get("nowindowstate", WindowState.None);
		
			
			foreach (IViewContent view in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (IsStateOk(view.WorkbenchWindow)) {
					return true;
				}
			}
			
			return false;
			
		}
	}
}
