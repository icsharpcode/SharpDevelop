// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Tests if any open window has a specified window state.
	/// </summary>
	public class OpenWindowStateConditionEvaluator : IConditionEvaluator
	{
		WindowState windowState = WindowState.None;
		WindowState nowindowState = WindowState.None;
		
		bool IsStateOk(IViewContent viewContent)
		{
			if (viewContent == null) {
				return false;
			}
			// use IWorkbenchWindow instead of IViewContent because maybe window info is needed in the future (for example: sub view content info.)
			bool isWindowStateOk = false;
			if (windowState != WindowState.None) {
				if ((windowState & WindowState.Dirty) > 0) {
					isWindowStateOk |= viewContent.IsDirty;
				}
				if ((windowState & WindowState.Untitled) > 0) {
					isWindowStateOk |= IsUntitled(viewContent);
				}
				if ((windowState & WindowState.ViewOnly) > 0) {
					isWindowStateOk |= viewContent.IsViewOnly;
				}
			} else {
				isWindowStateOk = true;
			}
			
			if (nowindowState != WindowState.None) {
				if ((nowindowState & WindowState.Dirty) > 0) {
					isWindowStateOk &= !viewContent.IsDirty;
				}
				
				if ((nowindowState & WindowState.Untitled) > 0) {
					isWindowStateOk &= !IsUntitled(viewContent);
				}
				
				if ((nowindowState & WindowState.ViewOnly) > 0) {
					isWindowStateOk &= !viewContent.IsViewOnly;
				}
			}
			return isWindowStateOk;
		}
		
		static bool IsUntitled(IViewContent viewContent)
		{
			OpenedFile file = viewContent.PrimaryFile;
			if (file == null)
				return false;
			else
				return file.IsUntitled;
		}
		
		public bool IsValid(object caller, Condition condition)
		{
			if (WorkbenchSingleton.Workbench == null) {
				return false;
			}
			
			windowState   = condition.Properties.Get("openwindowstate", WindowState.None);
			nowindowState = condition.Properties.Get("noopenwindowstate", WindowState.None);
			
			
			foreach (IViewContent view in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (IsStateOk(view)) {
					return true;
				}
			}
			
			return false;
			
		}
	}
}
