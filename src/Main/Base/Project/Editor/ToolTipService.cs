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
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// Static class for the ToolTipRequested event.
	/// </summary>
	public static class ToolTipRequestService
	{
		const string ToolTipProviderAddInTreePath = "/SharpDevelop/ViewContent/TextEditor/ToolTips";
		
		/// <summary>
		/// This event occurs on a tool tip request,
		/// after any <see cref="ITextAreaToolTipProvider"/> registered in the addintree have run.
		/// This event is still raised if AddIns handled it, so please check the Handled property.
		/// </summary>
		public static event EventHandler<ToolTipRequestEventArgs> ToolTipRequested;
		
		public static void RequestToolTip(ToolTipRequestEventArgs e)
		{
			if (e == null)
				throw new ArgumentNullException("e");
			
			if (!CodeCompletionOptions.TooltipsEnabled) {
				e.Handled = true;
				return;
			}
			
			if (CodeCompletionOptions.TooltipsOnlyWhenDebugging) {
				if (!SD.Debugger.IsDebuggerLoaded || !SD.Debugger.IsDebugging) {
					e.Handled = true;
					return;
				}
			}
			
			// Query all registered tooltip providers using the AddInTree.
			// The first one that does not return null will be used.
			foreach (ITextAreaToolTipProvider toolTipProvider in AddInTree.BuildItems<ITextAreaToolTipProvider>(ToolTipProviderAddInTreePath, null, false)) {
				toolTipProvider.HandleToolTipRequest(e);
				if (e.Handled)
					break;
			}
			
			EventHandler<ToolTipRequestEventArgs> eh = ToolTipRequested;
			if (eh != null)
				eh(null, e);
		}
	}
}
