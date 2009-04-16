// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Debugging;
using System;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;

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
	
	public class ToolTipRequestEventArgs : EventArgs
	{
		/// <summary>
		/// Gets whether the tool tip request was handled.
		/// </summary>
		public bool Handled { get; set; }
		
		/// <summary>
		/// Gets the editor causing the request.
		/// </summary>
		public ITextEditor Editor { get; private set; }
		
		/// <summary>
		/// Gets whether the mouse was inside the document bounds.
		/// </summary>
		public bool InDocument { get; set; }
		
		/// <summary>
		/// The mouse position, in document coordinates.
		/// </summary>
		public Location LogicalPosition { get; set; }
		
		/// <summary>
		/// Gets/Sets the content to show as a tooltip.
		/// </summary>
		public object ContentToShow { get; set; }
		
		/// <summary>
		/// Shows the tool tip.
		/// </summary>
		public void ShowToolTip(object content)
		{
			if (content == null)
				throw new ArgumentNullException("content");
			this.Handled = true;
			this.ContentToShow = content;
		}
		
		public ToolTipRequestEventArgs(ITextEditor editor)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			this.Editor = editor;
			this.InDocument = true;
		}
	}
}
