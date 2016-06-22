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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

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
			if (SD.Workbench == null) {
				return false;
			}
			
			windowState   = condition.Properties.Get("openwindowstate", WindowState.None);
			nowindowState = condition.Properties.Get("noopenwindowstate", WindowState.None);
			
			
			foreach (IViewContent view in SD.Workbench.ViewContentCollection) {
				if (IsStateOk(view)) {
					return true;
				}
			}
			
			return false;
			
		}
	}
}
