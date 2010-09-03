// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		const string ToolTipProviderAddInTreePath = "/SharpDevelop/ViewContent/DefaultTextEditor/ToolTips";
		
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
			
			if (!CodeCompletionOptions.EnableCodeCompletion) return;
			if (!CodeCompletionOptions.TooltipsEnabled) return;
			
			if (CodeCompletionOptions.TooltipsOnlyWhenDebugging) {
				if (!DebuggerService.IsDebuggerLoaded) return;
				if (!DebuggerService.CurrentDebugger.IsDebugging) return;
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
